using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GLP.Basecode.API.Voting.Services
{
    public class ImageFilePath
    {
        private readonly IWebHostEnvironment _env;

        public ImageFilePath(IWebHostEnvironment env)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public byte[] SaveAsPNG(IFormFile picture)
        {
            if (picture.ContentType != "image/png" &&
                picture.ContentType != "image/jpeg" &&
                picture.ContentType != "image/jpg")
            {
                throw new InvalidDataException("Invalid image format. Only PNG or JPEG is allowed.");
            }

            using var memoryStream = new MemoryStream();
            picture.CopyTo(memoryStream);

            using var imageStream = new MemoryStream(memoryStream.ToArray());
            using var image = Image.FromStream(imageStream);

            using var pngStream = new MemoryStream();
            image.Save(pngStream, ImageFormat.Png);

            return pngStream.ToArray();
        }

        public string SaveImageInFolder(byte[] imageData, string schoolYear, string rootFolder, string folderName)
        {
            try
            {
                if (_env.WebRootPath == null)
                {
                    throw new InvalidOperationException("WebRootPath is null. Ensure the application is properly configured and WebRootPath is set.");
                }

                string rootPath = Path.Combine(_env.WebRootPath, "File", "Images", schoolYear, rootFolder, folderName);
                Directory.CreateDirectory(rootPath); // create if not exists

                string fileName = Guid.NewGuid().ToString() + ".png";
                string fullPath = Path.Combine(rootPath, fileName);

                File.WriteAllBytes(fullPath, imageData);

                // Return relative path to be stored in DB or used in frontend
                return Path.Combine("/File/Images", folderName, fileName).Replace("\\", "/");
            }
            catch (Exception ex)
            {
                // Optionally log the error here
                throw new ApplicationException("An error occurred while saving the image.", ex);
            }
        }




        public bool DeleteImage(string relativePath)
        {
            string fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return !File.Exists(fullPath);
            }

            return false;
        }
    }
}
