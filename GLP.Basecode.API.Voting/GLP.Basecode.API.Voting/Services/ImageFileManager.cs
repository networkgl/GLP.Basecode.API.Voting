using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using GLP.Basecode.API.Voting.Models;
using GLP.Basecode.API.Voting.Handler;

namespace GLP.Basecode.API.Voting.Services
{
    public class ImageFileManager
    {
        private readonly IWebHostEnvironment _env;
        private readonly ExceptionHandlerMessage _exceptionHandlerMessage;

        public ImageFileManager(IWebHostEnvironment env, ExceptionHandlerMessage exceptionHandlerMessage)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _exceptionHandlerMessage = exceptionHandlerMessage;
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


        //CANDIDATE 
        public (bool IsSaved, string? RelativePath, string? ErrMsg) SaveImageToCandidateFolder(
            byte[] imageData,
            string schoolYear,
            string rootFolder,
            string partyListName,
            string candidateName,
            string posName)
        {
            try
            {
                if (_env.WebRootPath == null)
                {
                    throw new InvalidOperationException("WebRootPath is not set.");
                }

                string folderPath = Path.Combine(_env.WebRootPath, "File", "Images", schoolYear, rootFolder, partyListName, "Group Image", "Candidates");
                Directory.CreateDirectory(folderPath);

                string fileName = string.Join("-",candidateName, posName) + ".png";
                string fullPath = Path.Combine(folderPath, fileName);

                File.WriteAllBytes(fullPath, imageData);

                string relativePath = Path.Combine("File", "Images", schoolYear, rootFolder, partyListName, "Group Image", "Candidates", fileName)
                                      .Replace("\\", "/");

                return (true, "/" + relativePath, null);
            }
            catch (Exception e)
            {
                return (false, null, _exceptionHandlerMessage.GetInnermostExceptionMessage(e));
            }
        }


        //PARTY LIST 
        // tested
        public (bool IsSaved, string? RelativePath, string? ErrMsg) SaveImageToPartyListFolder(
            byte[] imageData,
            string schoolYear,
            string rootFolder,
            string partyListName)
        {
            try
            {
                if (_env.WebRootPath == null)
                {
                    throw new InvalidOperationException("WebRootPath is not set.");
                }

                string folderPath = Path.Combine(_env.WebRootPath, "File", "Images", schoolYear, rootFolder, partyListName, "Group Image");
                Directory.CreateDirectory(folderPath);

                string fileName = partyListName + ".png";
                string fullPath = Path.Combine(folderPath, fileName);

                File.WriteAllBytes(fullPath, imageData);

                string relativePath = Path.Combine("File", "Images", schoolYear, rootFolder, partyListName, "Group Image", fileName)
                                      .Replace("\\", "/");

                return (true, "/" + relativePath, null);
            }
            catch (Exception e)
            {
                return (false, null, _exceptionHandlerMessage.GetInnermostExceptionMessage(e));
            }
        }
        
        // tested
        public (bool Success, string? NewRelativePath, string? FileName, string? ErrMsg) RenameFolderPartyList(
            string schoolYear,
            string rootFolder,
            string oldFolderName,
            string newFolderName)
        {
            try
            {
                string partyListRoot = Path.Combine(_env.WebRootPath, "File", "Images", schoolYear, rootFolder);

                string oldPath = Path.Combine(partyListRoot, oldFolderName);
                string newPath = Path.Combine(partyListRoot, newFolderName);

                if (!Directory.Exists(oldPath))
                    return (false, null, null, "Old path does not exist.");

                if (Directory.Exists(newPath))
                    return (false, null, null, "New path already exists.");

                // Rename the folder (e.g., PINS Party List → wews123)
                Directory.Move(oldPath, newPath);

                // Access image inside Group Image folder (if it exists)
                string groupImagePath = Path.Combine(newPath, "Group Image");
                if (!Directory.Exists(groupImagePath))
                    return (false, null, null, "Group Image folder not found in renamed folder.");

                var imageFiles = Directory.GetFiles(groupImagePath);
                if (imageFiles.Length == 0)
                    return (false, null, null, "No image found inside Group Image folder.");

                string fileName = Path.GetFileName(imageFiles[0]);

                string relativePath = Path.Combine("File", "Images", schoolYear, rootFolder, newFolderName, "Group Image")
                                       .Replace("\\", "/");

                return (true, "/" + relativePath, fileName, "success");
            }
            catch (Exception ex)
            {
                return (false, null, null, _exceptionHandlerMessage.GetInnermostExceptionMessage(ex));
            }
        }
       
        // tested
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
