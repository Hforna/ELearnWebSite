using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Application.Services
{
    public class MapperService : Profile
    {
        public MapperService()
        {
            RequestToEntity();
            EntityToResponse();
        }

        private void RequestToEntity()
        {
            
        }

        private void EntityToResponse()
        {

        }
    }
}
