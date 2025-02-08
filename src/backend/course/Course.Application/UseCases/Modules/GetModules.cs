using AutoMapper;
using Course.Communication.Responses;
using Course.Domain.Repositories;
using Course.Domain.Services.Rest;
using Course.Exception;
using Org.BouncyCastle.Asn1.Mozilla;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Modules
{
    public class GetModules : IGetModules
    {
        private readonly ILinkService _linkService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uof;
        private readonly SqidsEncoder<long> _sqids;

        public GetModules(IMapper mapper, ILinkService linkService, IUnitOfWork unitOfWork, SqidsEncoder<long> sqidsEncoder)
        {
            _mapper = mapper;
            _linkService = linkService;
            _uof = unitOfWork;
            _sqids = sqidsEncoder;
        }

        public async Task<ModulesResponse> Execute(long courseId)
        {
            var course = await _uof.courseRead.CourseById(courseId, true);

            if (course is null)
                throw new CourseException(ResourceExceptMessages.COURSE_DOESNT_EXISTS);

            var response = _mapper.Map<ModulesResponse>(course.Modules);
            response.Modules.Select(module =>
            {
                module.AddLink("lessons", _linkService.GenerateResourceLink("GetLessons", new { id = module.Id }), "GET");

                return module;
            });
            response.CourseId = _sqids.Encode(course.Id);
            return response;
        }
    }
}
