using AutoMapper;
using Course.Application.Services;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Domain.Services.Rest;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Course.Application.AppServices
{
    public interface ILessonService
    {
        Task<LessonResponse> CreateLessonAsync(CreateLessonRequest request, long moduleId, long courseId);

        Task<LessonResponse> GetLessonAsync(long courseId, long moduleId, long id);

        Task<LessonsResponse> GetLessonsAsync(long moduleId, long courseId);

        Task<LessonShortInfosResponse> GetLessonShortInfosAsync(long lessonId);

        Task<LessonResponse> UpdateLessonAsync(long courseId, long moduleId, long id, UpdateLessonRequest request);

        Task DeleteLessonAsync(long courseId, long moduleId, long id);
    }

    public class LessonService : ILessonService
    {
        private readonly SqidsEncoder<long> _sqids;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _uof;
        private readonly FileService _fileService;

        public LessonService(
            SqidsEncoder<long> sqids,
            IMapper mapper,
            IUserService userService,
            IStorageService storageService,
            IUnitOfWork uof,
            FileService fileService)
        {
            _sqids = sqids;
            _mapper = mapper;
            _fileService = fileService;
            _userService = userService;
            _storageService = storageService;
            _uof = uof;
        }

        public async Task<LessonResponse> CreateLessonAsync(CreateLessonRequest request, long moduleId, long courseId)
        {
            var module = await _uof.moduleRead.ModuleById(moduleId);

            if (module is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            if (module.Course.TeacherId != userId)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var lesson = _mapper.Map<Lesson>(request);
            lesson.ModuleId = module.Id;

            var isVideo = _fileService.IsVideo(request.Video);

            if (!isVideo)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var notTranscodedStream = request.Video.OpenReadStream();
            (Stream videoStream, string fileName, string tempIntput, string tempOutput) = await _fileService.TranscodeVideo(notTranscodedStream);

            var videoDuration = await _fileService.GetFileDuration(videoStream, ".mp4");

            lesson.Duration = videoDuration;
            module.Duration += videoDuration;
            module.Course.Duration += videoDuration;

            _uof.courseWrite.UpdateCourse(module.Course);
            _uof.moduleWrite.UpdateModule(module);

            var video = new Video()
            {
                IsTranscoded = true,
            };

            if (request.Thumbnail is not null)
            {
                var thumbnail = request.Thumbnail.OpenReadStream();
                (bool isValid, string ext) = _fileService.ValidateImage(thumbnail);

                if (!isValid)
                    throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

                var thumbnailName = $"{Guid.NewGuid()}{ext}";
                video.thumbnailName = thumbnailName;

                await _uof.videoWrite.AddVideo(video);
                await _uof.Commit();

                await _storageService.UploadThumbnailVideo(video.Id, thumbnailName, thumbnail);
            }
            else
            {
                await _uof.videoWrite.AddVideo(video);
                await _uof.Commit();
            }

            lesson.VideoId = video.Id;

            _uof.lessonWrite.AddLesson(lesson);
            await _uof.Commit();

            await _storageService.UploadVideo(module.Course.courseIdentifier, videoStream, video.Id);

            File.Delete(tempIntput);
            File.Delete(tempOutput);

            var response = _mapper.Map<LessonResponse>(lesson);
            response.VideoUrl = await _storageService.GetVideo(module.Course.courseIdentifier, video.Id);

            return response;
        }

        public async Task<LessonResponse> GetLessonAsync(long courseId, long moduleId, long id)
        {
            var lesson = await _uof.lessonRead.LessonByModuleIdAndCourseId(moduleId, courseId, id);
            if (lesson is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var user = await _userService.GetUserInfos();
            if ((user is null && !lesson.isFree) ||
                (user is not null &&
                (await _uof.enrollmentRead.UserGotCourse(courseId, _sqids.Decode(user.id).Single())
                || lesson.isFree)))
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var response = _mapper.Map<LessonResponse>(lesson);
            response.VideoUrl = await _storageService.GetVideo(lesson.Module.Course.courseIdentifier, lesson.VideoId);

            return response;
        }

        public async Task<LessonsResponse> GetLessonsAsync(long moduleId, long courseId)
        {
            var lesson = await _uof.lessonRead.LessonsByModuleIdAndCourseId(moduleId, courseId);
            if (lesson is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var module = lesson.FirstOrDefault()!.Module;
            var lessons = module.Lessons;

            var user = await _userService.GetUserInfos();
            if (user is null || await _uof.enrollmentRead.UserGotCourse(module.CourseId, _sqids.Decode(user.id).Single()) == false)
                lessons.Where(d => d.isFree);

            var response = _mapper.Map<LessonsResponse>(lessons);
            var responseLessons = response.Lessons.Select(async l =>
            {
                l.VideoUrl = await _storageService.GetVideo(module.Course.courseIdentifier, l.VideoId);
                return l;
            });

            var lessonsList = await Task.WhenAll(responseLessons);
            response.ModuleId = _sqids.Encode(module.Id);
            response.Lessons = lessonsList.ToList();

            return response;
        }

        public async Task<LessonShortInfosResponse> GetLessonShortInfosAsync(long lessonId)
        {
            var lesson = await _uof.lessonRead.LessonById(lessonId);
            if (lesson is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var response = _mapper.Map<LessonShortInfosResponse>(lesson);
            return response;
        }

        public async Task<LessonResponse> UpdateLessonAsync(long courseId, long moduleId, long id, UpdateLessonRequest request)
        {
            var lesson = await _uof.lessonRead.LessonByModuleIdAndCourseId(moduleId, courseId, id);
            if (lesson is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var course = lesson.Module.Course;
            var module = lesson.Module;

            ValidateUpdateLesson(request, lesson, module);

            var requestModuleDecoded = _sqids.Decode(request.ModuleId).Single();
            var Requestedmodule = module.Id != requestModuleDecoded ? await _uof.moduleRead.ModuleById(requestModuleDecoded) : module;

            if (Requestedmodule is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            if (course.TeacherId != userId)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            _mapper.Map(request, lesson);
            lesson.ModuleId = requestModuleDecoded;
            _uof.lessonWrite.UpdateLesson(lesson);

            if (Requestedmodule.Id == module.Id)
            {
                module.Lessons.Remove(lesson);
                module.Lessons.Insert(request.Order, lesson);
            }
            else
            {
                Requestedmodule.Lessons.Insert(request.Order, lesson);
                module.Duration -= lesson.Duration;
                Requestedmodule.Duration += lesson.Duration;
                _uof.moduleWrite.UpdateModule(Requestedmodule);
            }

            _uof.moduleWrite.UpdateModule(module);
            await _uof.Commit();

            return _mapper.Map<LessonResponse>(lesson);
        }

        public async Task DeleteLessonAsync(long courseId, long moduleId, long id)
        {
            var lesson = await _uof.lessonRead.LessonByModuleIdAndCourseId(moduleId, courseId, id);
            if (lesson is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            if (lesson.Module.Course.TeacherId != userId)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var video = await _uof.videoRead.VideoById(lesson.VideoId);
            if (video is null)
                return;

            var course = lesson.Module.Course;
            var module = lesson.Module;

            await _storageService.DeleteVideo(course.courseIdentifier, video.Id);

            course.Duration -= lesson.Duration;
            module.Duration -= lesson.Duration;

            await _uof.videoWrite.DeleteVideo(video.Id);
            _uof.lessonWrite.DeleteLesson(lesson);
            _uof.courseWrite.UpdateCourse(course);
            _uof.moduleWrite.UpdateModule(module);

            await _uof.Commit();
        }

        private void ValidateUpdateLesson(UpdateLessonRequest request, Lesson lesson, Module module)
        {
            if (request.Order > module.Lessons.Count || request.Order <= 0)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);

            var decodeRequestModule = _sqids.Decode(request.ModuleId);
            if (!decodeRequestModule.Any())
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_IN_WISH_LIST);
        }
    }
}