using FileTypeChecker.Extensions;
using FileTypeChecker.Types;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Utilities.Zlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace Course.Application.Services
{
    public static class FileService
    {
        public static (bool isValid, string ext) ValidateImage(Stream file)
        {
            (bool isValid, string ext) = (false, "");

            if(file.Is<JointPhotographicExpertsGroup>())
            {
                (isValid, ext) = (true, GetExtension(JointPhotographicExpertsGroup.TypeExtension));
            } else if(file.Is<PortableNetworkGraphic>())
            {
                (isValid, ext) = (true, GetExtension(PortableNetworkGraphic.TypeExtension));
            }

            file.Position = 0;

            return (isValid, ext);
        }

        private static string GetExtension(string ext)
        {
            return ext.StartsWith(".") ? ext : $".{ext}";
        }
    }
}
