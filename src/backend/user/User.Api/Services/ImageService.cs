using FileTypeChecker;
using FileTypeChecker.Extensions;
using FileTypeChecker.Types;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace User.Api.Services
{
    public class ImageService
    {
        public (bool isImage, string ext) ValidateImage(Stream file)
        {
            (bool isImage, string ext) = (false, "");

            if (file.Is<JointPhotographicExpertsGroup>())
            {
                (isImage, ext) = (true, GetExtension(JointPhotographicExpertsGroup.TypeExtension));
            } else if(file.Is<PortableNetworkGraphic>())
            {
                (isImage, ext) = (true, GetExtension(PortableNetworkGraphic.TypeExtension));
            }

            file.Position = 0;

            return (isImage, ext);
        }

        private string GetExtension(string extension)
        {
            return extension.StartsWith(".") ? extension : $".{extension}";
        }
    }
}
