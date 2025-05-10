using Progress.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Domain.Rest
{
    public interface IUserRestService
    {
        public Task<UserInfosDto> GetUserInfos();
    }
}
