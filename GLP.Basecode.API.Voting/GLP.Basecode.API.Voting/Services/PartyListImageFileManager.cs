using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using GLP.Basecode.API.Voting.Models;

namespace GLP.Basecode.API.Voting.Services
{
    public class PartyListImageFileManager
    {
        private readonly IWebHostEnvironment _env;

        public PartyListImageFileManager(IWebHostEnvironment env)
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

        public string SaveImageInFolderAsCreatePartyList(byte[] imageData, string schoolYear, string rootFolder, string folderName)
        {
            
            try
            {
                if (_env.WebRootPath == null)
                {
                    throw new InvalidOperationException("WebRootPath is null. Ensure the application is properly configured and WebRootPath is set.");
                }

                string rootPath = Path.Combine(_env.WebRootPath, "File", "Images", schoolYear, rootFolder, "Group Image", folderName);
                Directory.CreateDirectory(rootPath); // create if not exists

                string fileName = Guid.NewGuid().ToString() + ".png";
                string fullPath = Path.Combine(rootPath, fileName);

                File.WriteAllBytes(fullPath, imageData);

                // Return relative path to be stored in DB or used in frontend
                return Path.Combine("File","Images", schoolYear,rootFolder, folderName, "Group Image", fileName).Replace("\\", "/");
            }
            catch (Exception ex)
            {
                // Optionally log the error here
                throw new ApplicationException("An error occurred while saving the image.", ex);
            }
        }


        public (bool Success, string? NewRelativePath, string? FileName) RenameFolder(
            string schoolYear,
            string rootFolder,
            string oldFolderName,
            string newFolderName)
        {
            try
            {
                string baseDirectory = Path.Combine(_env.WebRootPath, "File", "Images", schoolYear, rootFolder, "Group Image");
                string oldPath = Path.Combine(baseDirectory, oldFolderName);
                string newPath = Path.Combine(baseDirectory, newFolderName);

                if (!Directory.Exists(oldPath) || Directory.Exists(newPath))
                {
                    return (false, null, null);
                }

                Directory.Move(oldPath, newPath);

                var imageFiles = Directory.GetFiles(newPath);
                if (imageFiles.Length == 0)
                {
                    return (false, null, null); // No image found
                }

                string fileName = Path.GetFileName(imageFiles[0]);
                string relativePath = Path.Combine("File", "Images", schoolYear, rootFolder, "Group Image", newFolderName)
                                      .Replace("\\", "/");

                return (true, "/" + relativePath, fileName);
            }
            catch (Exception)
            {
                // Optionally add logging here
                return (false, null, null);
            }
        }

        public (bool IsSaved, string RelativePath) SaveImageToFullPath(
            byte[] imageData,
            string schoolYear,
            string rootFolder,
            string folderName)
        {
            try
            {
                if (_env.WebRootPath == null)
                {
                    throw new InvalidOperationException("WebRootPath is not set.");
                }

                string folderPath = Path.Combine(_env.WebRootPath, "File", "Images", schoolYear, rootFolder, "Group Image", folderName);
                Directory.CreateDirectory(folderPath); // Ensures folder exists

                string fileName = Guid.NewGuid().ToString() + ".png";
                string fullImagePath = Path.Combine(folderPath, fileName);

                File.WriteAllBytes(fullImagePath, imageData);

                string relativePath = Path.Combine("File", "Images", schoolYear, rootFolder, folderName, "Group Image", fileName)
                                      .Replace("\\", "/");

                return (true, "/" + relativePath);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to save the image to full path.", ex);
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






        //public (bool Success, string? NewRelativePath, string? fileName) RenameFolder(
        //     string schoolYear,
        //     string rootFolder,
        //     string oldFolderName,
        //     string newFolderName)
        //{
        //    try
        //    {
        //        string root = Path.Combine(_env.WebRootPath, "File", "Images", schoolYear, rootFolder);
        //        string oldPath = Path.Combine(root, oldFolderName);
        //        string newPath = Path.Combine(root, newFolderName);

        //        if (Directory.Exists(oldPath) && !Directory.Exists(newPath))
        //        {
        //            Directory.Move(oldPath, newPath);

        //            var files = Directory.GetFiles(newPath);
        //            if (files.Length == 0)
        //                return (false, null, null); // no image found

        //            var fileName = Path.GetFileName(files[0]);

        //            //string relativePath = Path.Combine("File", "Images", schoolYear, rootFolder, newName, fileName)
        //            //                        .Replace("\\", "/");

        //            string relativePath = Path.Combine("File", "Images", schoolYear, rootFolder, newFolderName)
        //                                    .Replace("\\", "/");

        //            return (true, "/" + relativePath, fileName);
        //        }

        //        return (false, null, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Optional logging
        //        return (false, null, null);
        //    }
        //}
        //public (bool isSaved, string newRelativePath) SaveImageToFullPath(byte[] imageData, string schoolYear, string rootFolder, string partyListName)
        //{
        //    try
        //    {
        //        if (_env.WebRootPath == null)
        //        {
        //            throw new InvalidOperationException("WebRootPath is null. Ensure the application is properly configured and WebRootPath is set.");
        //        }

        //        string rootPath = Path.Combine(_env.WebRootPath, "File", "Images", schoolYear, rootFolder, partyListName);

        //        Directory.CreateDirectory(rootPath); // create if not exists

        //        string fileName = Guid.NewGuid().ToString() + ".png";
        //        string fullPath = Path.Combine(rootPath, fileName);

        //        File.WriteAllBytes(fullPath, imageData);
        //        string relativePath = Path.Combine("File", "Images", schoolYear, rootFolder, partyListName, fileName)
        //                                .Replace("\\", "/");
        //        return (true, "/" + relativePath);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException("An error occurred while saving the image.", ex);
        //    }
        //}


    }
}
