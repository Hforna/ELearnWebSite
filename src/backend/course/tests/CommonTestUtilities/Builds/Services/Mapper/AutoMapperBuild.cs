using AutoMapper;
using Course.Application.Services.AutoMapper;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTestUtilities.Builds.Services.Mapper
{
    public static class AutoMapperBuild
    {
        public static IMapper Build()
        {
            var sqids = SqidsBuild.Build();
            return new MapperConfiguration(x => { x.AddProfile(new MapperService(sqids)); }).CreateMapper();
        }
    }
}
