using AutoMapper;
using Payment.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUtilities.Builds.Mapper
{
    public static class AutoMapperMock
    {
        public static IMapper Build()
        {
            var sqids = SqidsMock.Build();
            return new MapperConfiguration(x => { x.AddProfile(new MapperService(sqids)); }).CreateMapper();
        }
    }
}
