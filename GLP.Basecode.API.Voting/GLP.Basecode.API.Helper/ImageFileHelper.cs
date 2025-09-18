using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;


namespace GLP.Basecode.API.Helper
{
    public class ImageFileHelper
    {
        private readonly IWebHostEnvironment _env;
        private readonly ExceptionMessageHelper _exceptionMessageHelper;

        public ImageFileHelper(IWebHostEnvironment env, ExceptionMessageHelper exceptionMessageHelper)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _exceptionMessageHelper = exceptionMessageHelper;
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

                string folderPath = Path.Combine(_env.WebRootPath, "File", "Images", schoolYear, rootFolder, partyListName, "Candidates");
                Directory.CreateDirectory(folderPath);

                string fileName = string.Join(" - ",candidateName, posName) + ".png";
                string fullPath = Path.Combine(folderPath, fileName);

                File.WriteAllBytes(fullPath, imageData);

                string relativePath = Path.Combine("File", "Images", schoolYear, rootFolder, partyListName, "Candidates", fileName)
                                      .Replace("\\", "/");

                return (true, "/" + relativePath, null);
            }
            catch (Exception e)
            {
                return (false, null, _exceptionMessageHelper.GetInnermostExceptionMessage(e));
            }
        }


        public (bool Success, string? NewRelativePath, string? ErrMsg) RenameCanImgFileName(
            string schoolYear,
            string rootFolder,
            string partyListName,
            string oldRelativePath,
            string candidateName,
            string candidatePosition)
        {
            try
            {
                // Build full absolute path of the old image
                string oldFullPath = Path.Combine(_env.WebRootPath, oldRelativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (!File.Exists(oldFullPath))
                {
                    return (false, null, $"File not found: {oldFullPath}");
                }

                // Get directory and build new file name
                string directory = Path.GetDirectoryName(oldFullPath)!;
                string newFileName = $"{candidateName} - {candidatePosition}.png";
                string newFullPath = Path.Combine(directory, newFileName);

                if (File.Exists(newFullPath))
                {
                    return (false, null, "A file with the new name already exists.");
                }

                File.Move(oldFullPath, newFullPath);

                // Build relative path to return
                string relativePath = newFullPath.Replace(_env.WebRootPath, "").Replace("\\", "/");
                return (true, relativePath, "success");
            }
            catch (Exception ex)
            {
                return (false, null, _exceptionMessageHelper.GetInnermostExceptionMessage(ex));
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
                return (false, null, _exceptionMessageHelper.GetInnermostExceptionMessage(e));
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
                return (false, null, null, _exceptionMessageHelper.GetInnermostExceptionMessage(ex));
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
