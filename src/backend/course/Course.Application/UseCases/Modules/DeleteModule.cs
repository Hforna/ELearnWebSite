using Course.Application.UseCases.Repositories.Modules;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Domain.Services.Rest;
using Course.Exception;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.UseCases.Modules
{
    public class DeleteModule : IDeleteModule
    {
        private readonly IUnitOfWork _uof;
        private readonly IUserService _userService;
        private readonly SqidsEncoder<long> _sqids;
        private readonly IStorageService _storage;

        public DeleteModule(IUnitOfWork uof, IUserService userService, 
            SqidsEncoder<long> sqids, IStorageService storage)
        {
            _uof = uof;
            _userService = userService;
            _sqids = sqids;
            _storage = storage;
        }

        public async Task Execute(long id)
        {
            var module = await _uof.moduleRead.ModuleById(id);

            if (module is null)
                throw new CourseException(ResourceExceptMessages.MODULE_DOESNT_EXISTS);

            var user = await _userService.GetUserInfos();
            var userId = _sqids.Decode(user.id).Single();

            if (userId != module.Course.TeacherId)
                throw new CourseException(ResourceExceptMessages.MODULE_NOT_OF_USER);

            module.Active = false;

            _uof.moduleWrite.UpdateModule(module);
            await _uof.Commit();            
        }
    }
}
