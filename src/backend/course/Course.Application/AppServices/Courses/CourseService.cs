using AutoMapper;
using Course.Application.Extensions;
using Course.Application.Services;
using Course.Application.Services.Validators.Course;
using Course.Application.Services.Validators.Quiz;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.Cache;
using Course.Domain.DTOs;
using Course.Domain.Entitites;
using Course.Domain.Enums;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Domain.Services.RabbitMq;
using Course.Domain.Services.Rest;
using Course.Domain.Sessions;
using Course.Exception;
using FluentValidation;
using SharedMessages.CourseMessages;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Courses
{
    public interface ICourseService
    {
        public Task<int> CourseLessonsCount(long courseId);
        public Task<CoursesPaginationResponse> CoursesUserBought(int page, int quantity);
        public Task<CourseShortResponse> CreateCourse(CreateCourseRequest request);
        public Task DeleteCourse(long id);
        public Task<CourseResponse> GetCourse(long id);
        public Task<CoursesPaginationResponse> GetCourses(GetCoursesRequest request, int page, int itemsQuantity);
        public Task<CoursesResponse> GetTenMostPopularWeekCourses();
        public Task<CoursesPaginationResponse> TeacherCourses(int page, int quantity, long teacherId);
        public Task<CourseShortResponse> UpdateCourse(long id, UpdateCourseRequest request);
        public Task<bool> UserGotCourse(GetCourseRequest request);
    }

    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _uof;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IStorageService _storageService;
        private readonly ILocationService _locationService;
        private readonly ICurrencyExchangeService _currencyExchange;
        private readonly FileService _fileService;
        private readonly IProducerService _producerService;
        private readonly EmailService _emailService;
        private readonly IDeleteCourseSender _deleteCourseSender;
        private readonly ICourseCache _courseCache;
        private readonly ICoursesSession _coursesSession;
        private readonly ILinkService _linkService;

        public CourseService(IUnitOfWork uof, SqidsEncoder<long> sqids, 
            IUserService userService, IMapper mapper, 
            IStorageService storageService, ILocationService locationService, 
            ICurrencyExchangeService currencyExchange, FileService fileService, 
            IProducerService producerService, EmailService emailService, 
            IDeleteCourseSender deleteCourseSender, ICourseCache courseCache, 
            ICoursesSession coursesSession, ILinkService linkService)
        {
            _uof = uof;
            _sqids = sqids;
            _userService = userService;
            _mapper = mapper;
            _storageService = storageService;
            _locationService = locationService;
            _currencyExchange = currencyExchange;
            _fileService = fileService;
            _producerService = producerService;
            _emailService = emailService;
            _deleteCourseSender = deleteCourseSender;
            _courseCache = courseCache;
            _coursesSession = coursesSession;
            _linkService = linkService;
        }

        public async Task<int> CourseLessonsCount(long courseId)
        {
            var course = await _uof.courseRead.CourseById(courseId);

            if (course is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_DOESNT_EXISTS);

            var countLessons = await _uof.lessonRead.CountTotalLessons(courseId);
            var countQuizzes = await _uof.quizRead.CountQuizzes(courseId);

            return countLessons + countQuizzes;
        }

        public async Task<CoursesPaginationResponse> CoursesUserBought(int page, int quantity)
        {
            var user = await _userService.GetUserInfos();

            if (user is null)
                throw new NotAuthenticatedException(ResourceExceptMessages.USER_INFOS_DOESNT_EXISTS);

            var userId = _sqids.Decode(user.id).Single();

            var userCourses = await _uof.courseRead.CoursesUserHas(page, quantity, userId);

            var coursesSelect = userCourses.Select(async course =>
            {
                var response = _mapper.Map<CourseShortResponse>(course);
                response.ThumbnailUrl = await _storageService.GetCourseImage(course.courseIdentifier, course.Thumbnail);

                return response;
            }).ToList();

            var coursesResponse = await Task.WhenAll(coursesSelect);

            return new CoursesPaginationResponse()
            {
                Count = userCourses.Count,
                Courses = coursesResponse.ToList(),
                IsFirstPage = userCourses.IsFirstPage,
                IsLastPage = userCourses.IsLastPage,
                PageNumber = userCourses.PageNumber,
                TotalItemCount = userCourses.TotalItemCount
            };
        }

        public async Task<CourseShortResponse> CreateCourse(CreateCourseRequest request)
        {
            Validate<CreateCourseValidator, CreateCourseRequest>(request);

            var userInfos = await _userService.GetUserInfos();

            if (userInfos is null)
                throw new NotAuthenticatedException(ResourceExceptMessages.USER_INFOS_DOESNT_EXISTS);

            var userCurrencyAsEnum = await UserCurrencyAsEnumExtension.GetCurrency(_locationService);

            var course = _mapper.Map<CourseEntity>(request);

            var convertPrice = await ConvertPrice(userCurrencyAsEnum, course.CurrencyType, request.Price);
            course.Price = convertPrice;
            course.TeacherId = _sqids.Decode(userInfos.id).Single();
            course.Active = false;

            if (request.ThumbnailImage is not null)
            {
                var thumbnail = request.ThumbnailImage.OpenReadStream();

                var imageValidator = _fileService.ValidateImage(thumbnail);

                if (imageValidator.isValid)
                {
                    course.Thumbnail = $"{Guid.NewGuid()}{imageValidator.ext}";

                    await _storageService.UploadCourseImage(thumbnail, course.Thumbnail, course.courseIdentifier);
                }
            }

            _uof.courseWrite.AddCourse(course);
            _uof.courseWrite.AddCourseTopics(course.TopicsCovered);

            await _uof.Commit();

            var message = new CourseCreatedMessage() { UserId = userInfos.id };

            await _producerService.SendCourseCreated(message);

            var response = _mapper.Map<CourseShortResponse>(course);
            response.Price = request.Price;
            response.CourseId = _sqids.Encode(course.Id);
            response.TeacherId = _sqids.Encode(course.TeacherId);

            return response;
        }

        public async Task DeleteCourse(long id)
        {
            var course = await _uof.courseRead.CourseById(id);

            if (course is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_DOESNT_EXISTS);

            UserInfosDto user = await _userService.GetUserInfos();
            long userId = _sqids.Decode(user.id).Single();

            if (course.TeacherId != userId)
                throw new UnauthorizedException(ResourceExceptMessages.COURSE_NOT_OF_USER);

            course.Active = false;
            course.IsPublish = false;

            if (!string.IsNullOrEmpty(course.Thumbnail)) await _storageService.DeleteCourseImage(course.courseIdentifier, course.Thumbnail);

            _uof.courseWrite.UpdateCourse(course);
            await _uof.Commit();

            await _deleteCourseSender.SendMessage(course.Id);

            await _emailService.SendEmail(user.userName, user.email, "Are you sure you want to delete this course?",
                "you have 2 months for get it back otherwise it will be removed and only people that already bought will have access");
        }

        public async Task<CourseResponse> GetCourse(long id)
        {
            var course = await _uof.courseRead.CourseById(id);

            if (course is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_DOESNT_EXISTS);

            var getCoursesList = _coursesSession.GetCoursesVisited() ?? [];

            var courseInList = getCoursesList.Contains(course.Id);

            if (!courseInList)
            {
                await _courseCache.SetCourseOnMostVisited(course.Id);
                await _coursesSession.AddCourseVisited(course.Id);
            }

            course.totalVisits += courseInList == false ? 1 : 0;
            _uof.courseWrite.UpdateCourse(course);
            await _uof.Commit();

            var response = _mapper.Map<CourseResponse>(course);

            var userCurrencyAsEnum = await UserCurrencyAsEnumExtension.GetCurrency(_locationService);
            var convertPrice = await ConvertPrice(course.CurrencyType, userCurrencyAsEnum, course.Price);

            response.Price = Math.Round(convertPrice, 2, MidpointRounding.AwayFromZero);

            response.AddLink("modules", _linkService.GenerateResourceLink("GetModules", new
            {
                courseId = _sqids.Encode(course.Id),
                id = _sqids.Encode(course.Id)
            }), "GET");
            response.TeacherProfile = $"https://localhost:8080/profile/{course.TeacherId}";
            //response.ThumbnailUrl = await _storage.GetCourseImage(course.courseIdentifier, course.Thumbnail);

            return response;
        }

        public async Task<CoursesPaginationResponse> GetCourses(GetCoursesRequest request, int page, int itemsQuantity)
        {
            var filterDto = _mapper.Map<GetCoursesFilterDto>(request);

            var getCoursesList = _coursesSession.GetCoursesVisited();

            List<string>? coursesTypes = null;

            if (getCoursesList is not null)
            {
                var visitedCourses = await _uof.courseRead.CourseByIds(getCoursesList);
                coursesTypes = visitedCourses!
                    .GroupBy(d => d.CourseType)
                    .ToDictionary(d => d.Key.ToString(), g => g.Count())
                    .OrderByDescending(d => d.Value)
                    .Select(d => d.Key).ToList();
            }
            var mostPopularCourses = await _courseCache.GetMostPopularCourses();
            List<CourseEntity>? reccomendedCourses = new List<CourseEntity>();

            if (mostPopularCourses is not null)
            {
                mostPopularCourses = mostPopularCourses.OrderByDescending(d => d.Value).ToDictionary();

                reccomendedCourses = await _uof.courseRead.GetCourseByUserVisitsAndMostVisited(filterDto, mostPopularCourses.Keys.ToList(), coursesTypes);
            }

            var courses = _uof.courseRead.GetCoursesPagination(page, filterDto, reccomendedCourses, itemsQuantity);

            var userCurrencyAsEnum = await UserCurrencyAsEnumExtension.GetCurrency(_locationService);

            var coursesToResponse = courses.Select(async course =>
            {
                var response = _mapper.Map<CourseShortResponse>(course);
                //response.ThumbnailUrl = await _storageService.GetCourseImage(course.courseIdentifier, course.Thumbnail);
                var convertPrice = course.Price;
                if (userCurrencyAsEnum != course.CurrencyType)
                    convertPrice = await ConvertPrice(course.CurrencyType, userCurrencyAsEnum, course.Price);
                response.Price = Math.Round(convertPrice, 2, MidpointRounding.AwayFromZero);

                return response;
            });

            var coursesResponse = await Task.WhenAll(coursesToResponse);

            var response = new CoursesPaginationResponse()
            {
                Count = courses.Count,
                Courses = coursesResponse.ToList(),
                IsFirstPage = courses.IsFirstPage,
                IsLastPage = courses.IsLastPage,
                PageNumber = courses.PageNumber,
                TotalItemCount = courses.TotalItemCount
            };

            return response;
        }

        public async Task<CoursesResponse> GetTenMostPopularWeekCourses()
        {
            var cacheCourses = await _courseCache.GetMostPopularCourses();
            if (cacheCourses is null)
                return new CoursesResponse();

            cacheCourses = cacheCourses.Take(10).ToDictionary(k => k.Key, v => v.Value);

            var courses = await _uof.courseRead.CourseByIds(cacheCourses.Keys.ToList());

            var userCurrencyAsEnum = await UserCurrencyAsEnumExtension.GetCurrency(_locationService);

            var courseTasks = courses.Select(async course =>
            {
                var response = _mapper.Map<CourseShortResponse>(course);
                response.ThumbnailUrl = await _storageService.GetCourseImage(course.courseIdentifier, course.Thumbnail);
                var convertPrice = course.Price;
                if (userCurrencyAsEnum != course.CurrencyType)
                    convertPrice = await ConvertPrice(course.CurrencyType, userCurrencyAsEnum, course.Price);
               
                response.Price = Math.Round(convertPrice, 2, MidpointRounding.AwayFromZero);

                return response;
            }).ToList();

            var taskCourseResponse = await Task.WhenAll(courseTasks);

            return new CoursesResponse { courses = taskCourseResponse.ToList() };
        }

        public async Task<CoursesPaginationResponse> TeacherCourses(int page, int quantity, long teacherId)
        {
            var courses = _uof.courseRead.TeacherCoursesPagination(page, quantity, teacherId);

            if (courses is null)
                throw new NotFoundException(ResourceExceptMessages.TEACHER_DOESNT_HAVE_COURSES);

            var userCurrencyAsEnum = await UserCurrencyAsEnumExtension.GetCurrency(_locationService);

            var coursesResponse = courses.Select(async course =>
            {
                var response = _mapper.Map<CourseShortResponse>(course);
                response.ThumbnailUrl = await _storageService.GetCourseImage(course.courseIdentifier, course.Thumbnail);
                var convertPrice = course.Price;

                if (course.CurrencyType != userCurrencyAsEnum)
                    convertPrice = await ConvertPrice(course.CurrencyType, userCurrencyAsEnum, course.Price);

                response.Price = Math.Round(convertPrice, 2, MidpointRounding.AwayFromZero);

                return response;
            });

            var courseResponseList = await Task.WhenAll(coursesResponse);

            return new CoursesPaginationResponse()
            {
                Count = courses.Count,
                Courses = courseResponseList.ToList(),
                IsFirstPage = courses.IsFirstPage,
                IsLastPage = courses.IsLastPage,
                PageNumber = courses.PageNumber,
                TotalItemCount = courses.TotalItemCount
            };
        }

        public async Task<CourseShortResponse> UpdateCourse(long id, UpdateCourseRequest request)
        {
            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var course = await _uof.courseRead.CourseByTeacherAndId(userId, id);

            if (course is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_NOT_OF_USER);

            _mapper.Map(request, course);

            if (request.Thumbnail is not null)
            {
                var image = request.Thumbnail.OpenReadStream();

                (bool isValid, string ext) = _fileService.ValidateImage(image);

                if (!isValid)
                    throw new RequestException(ResourceExceptMessages.INVALID_FORMAT_IMAGE);

                course.Thumbnail = $"{Guid.NewGuid()}{ext}";

                await _storageService.UploadCourseImage(image,
                    course.Thumbnail
                    , course.courseIdentifier);
            }

            var response = _mapper.Map<CourseShortResponse>(course);
            response.ThumbnailUrl = string.IsNullOrEmpty(course.Thumbnail) == false ?
                await _storageService.GetCourseImage(course.courseIdentifier, course.Thumbnail)
                : "";

            _uof.courseWrite.UpdateCourse(course);
            await _uof.Commit();

            return response;
        }

        public async Task<bool> UserGotCourse(GetCourseRequest request)
        {
            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            var course = await _uof.courseRead.CourseById(_sqids.Decode(request.courseId).Single());

            if (course is null)
                throw new NotFoundException(ResourceExceptMessages.COURSE_DOESNT_EXISTS);

            var userGotCourse = await _uof.enrollmentRead.UserGotCourse(course.Id, userId);

            return userGotCourse;
        }


        void Validate<V, R>(R request) where V : AbstractValidator<R>
        {
            var validator = Activator.CreateInstance<V>();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                var errorMessages = result.Errors.Select(d => d.ErrorMessage).ToList();
                throw new RequestException(errorMessages);
            }
        }

        private async Task<double> ConvertPrice(Domain.Enums.CurrencyEnum sourceCurrency, CurrencyEnum targetCurrency, double price)
        {
            var rates = await _currencyExchange.GetCurrencyRates(sourceCurrency);

            return targetCurrency switch
            {
                CurrencyEnum.BRL => price * rates.BRL,
                CurrencyEnum.USD => price * rates.USD,
                CurrencyEnum.EUR => price * rates.EUR,
                _ => price
            };
        }

    }
}
