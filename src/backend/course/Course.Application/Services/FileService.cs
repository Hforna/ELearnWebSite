using FileTypeChecker.Extensions;
using FileTypeChecker.Types;
using MediaInfo.DotNetWrapper;
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
    public class FileService
    {
        public (bool isValid, string ext) ValidateImage(Stream file)
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

        public async Task<double> GetFileDuration(Stream file, string fileExtension)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{fileExtension}");

            using(var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fileStream);
            }

            var input = new MediaInfoWrapper(tempFilePath);
            var duration = Math.Ceiling(TimeSpan.FromMilliseconds(input.Duration).TotalMinutes);

            File.Delete(tempFilePath);

            return duration;
        }

        public async Task<(FileStream, string fileName, string intput, string output)> TranscodeVideo(Stream video)
        {
            var tempInput = Path.GetTempFileName();
            var tempOutput = Path.ChangeExtension(tempInput, ".mp4");

            using(var fileStream = new FileStream(tempInput, FileMode.Create, FileAccess.Write))
            {
                await video.CopyToAsync(fileStream);
            }

            var getMediaInfo = await FFmpeg.GetMediaInfo(tempInput);

            var conversion = FFmpeg.Conversions.New()
            .AddStream(getMediaInfo.VideoStreams.FirstOrDefault())
            .AddStream(getMediaInfo.AudioStreams.FirstOrDefault())
            .SetOutput(tempOutput)
            .AddParameter("-preset medium")
            .AddParameter("-c:v libx264");

            await conversion.Start();

            await using var outputFileStream = File.OpenRead(tempOutput);
            var fileName = Path.GetFileName(tempOutput);

            return (outputFileStream, fileName, tempInput, tempOutput);
        }

        private string GetExtension(string ext)
        {
            return ext.StartsWith(".") ? ext : $".{ext}";
        }
    }
}
