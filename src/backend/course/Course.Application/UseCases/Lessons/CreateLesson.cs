using AutoMapper;
using Course.Application.Services;
using Course.Application.UseCases.Repositories.Lessons;
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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Lessons
{
    public class CreateLesson : ICreateLesson
    {
        private readonly SqidsEncoder<long> _sqids;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _uof;
        private readonly FileService _fileService;

        public CreateLesson(SqidsEncoder<long> sqids, IMapper mapper, 
            IUserService userService, IStorageService storageService, 
            IUnitOfWork uof, FileService fileService)
        {
            _sqids = sqids;
            _mapper = mapper;
            _fileService = fileService;
            _userService = userService;
            _storageService = storageService;
            _uof = uof;
        }

        public async Task<LessonResponse> Execute(CreateLessonRequest request, long moduleId, long courseId)
        {
            var module = await _uof.moduleRead.ModuleById(moduleId);
        
            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();
        
            if (module.Course.TeacherId != userId)
                throw new LessonException(ResourceExceptMessages.COURSE_NOT_OF_USER);

            var lesson = _mapper.Map<Lesson>(request);
            lesson.ModuleId = module.Id;

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
                    throw new LessonException(ResourceExceptMessages.INVALID_FORMAT_IMAGE);

                var thumbnailName = $"{Guid.NewGuid()}{ext}";
                video.thumbnailName = thumbnailName;

                await _uof.videoWrite.AddVideo(video);
                await _uof.Commit();

                await _storageService.UploadThumbnailVideo(video.Id, thumbnailName, thumbnail);
            } else
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
    }
}
