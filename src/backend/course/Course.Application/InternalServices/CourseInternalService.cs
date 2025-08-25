using AutoMapper;
using Course.Communication.Responses;
using Course.Domain.Cache;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Domain.Services.Rest;
using Course.Domain.Sessions;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.InternalServices
{
    public interface ICourseInternalService
    {
        public Task<CourseResponse> CourseInternalServiceById(long id);
    }

    public class CourseInternalService : ICourseInternalService
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IStorageService _storage;
        private readonly ICoursesSession _coursesSession;
        private readonly ILinkService _linkService;
        private readonly ICourseCache _courseCache;
        private readonly ILocationService _locationService;
        private readonly ICurrencyExchangeService _exchangeService;

        public CourseInternalService(IUnitOfWork uof, IMapper mapper,
            SqidsEncoder<long> sqids, IStorageService storage,
            ICoursesSession coursesSession, ILinkService linkService,
            ICourseCache courseCache, ILocationService locationService, ICurrencyExchangeService exchangeService)
        {
            _uof = uof;
            _locationService = locationService;
            _linkService = linkService;
            _mapper = mapper;
            _exchangeService = exchangeService;
            _courseCache = courseCache;
            _sqids = sqids;
            _storage = storage;
            _coursesSession = coursesSession;
        }

        public async Task<CourseResponse> CourseInternalServiceById(long id)
        {
            var course = await _uof.courseRead.CourseById(id, true) 
                ?? throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS, System.Net.HttpStatusCode.NotFound);

            var response = _mapper.Map<CourseResponse>(course);
            response.AddLink("modules", _linkService.GenerateResourceLink("GetModules", new
            {
                courseId = _sqids.Encode(course.Id),
                id = _sqids.Encode(course.Id)
            }), "GET");
            response.TeacherProfile = $"https://localhost:8080/profile/{course.TeacherId}";
            //response.ThumbnailUrl = await _storage.GetCourseImage(course.courseIdentifier, course.Thumbnail);

            return response;
        }
    }
}
