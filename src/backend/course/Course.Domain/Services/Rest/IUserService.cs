using Course.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Services.Rest
{
    public interface IUserService
    {
        public Task<List<string>> GetUserRoles(Guid uid);
        public Task<UserInfosDto> GetUserInfos();
    }
}
