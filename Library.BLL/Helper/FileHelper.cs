using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.Helper
{
    public class FileHelper
    {
        public static async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            try
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot/", folderName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
                return fileName;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
