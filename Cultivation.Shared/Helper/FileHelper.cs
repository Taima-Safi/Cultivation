using Cultivation.Shared.Enum;
using Cultivation.Shared.Exception;
using Microsoft.AspNetCore.Http;

namespace Cultivation.FileHelper;

public class FileHelper
{
    public static string UploadFile(IFormFile file, FileType type)
    {
        List<string> validExtentions = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        var extention = Path.GetExtension(file.FileName);

        if (!validExtentions.Contains(extention))
            throw new InValidExtensionException();

        long size = file.Length;
        if (size >= (5 * 1024 * 1024)) // 5 mg
            throw new InValidSizeException();

        string fileName = Guid.NewGuid().ToString() + extention;

        string folderName = "";
        switch (type)
        {
            case FileType.Fertilizer:
                folderName = Path.Combine("Uploads", "Fertilizer");
                break;
            case FileType.Insecticide:
                folderName = Path.Combine("Uploads", "Insecticide");
                break;
            case FileType.InsecticideLand:
                folderName = Path.Combine("Uploads", "InsecticideLand");
                break;
        }
        string path = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        var fullPath = Path.Combine(path, fileName);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        using FileStream stream = new FileStream(fullPath, FileMode.Create);
        file.CopyTo(stream);

        return fileName;
    }
    public static async Task<MemoryStream> DownloadFile(string fileName, FileType type)
    {

        string folderName = "";
        switch (type)
        {
            case FileType.Fertilizer:
                folderName = Path.Combine("Uploads", "Fertilizer");
                break;
            case FileType.Insecticide:
                folderName = Path.Combine("Uploads", "Insecticide");
                break;
            case FileType.InsecticideLand:
                folderName = Path.Combine("Uploads", "InsecticideLand");
                break;
        }

        var path = Path.Combine(Directory.GetCurrentDirectory(), folderName, fileName);

        Console.WriteLine($"Constructed path: {path}");

        if (!System.IO.File.Exists(path))
            throw new NotFoundException("File does not exist");

        try
        {
            var f = new FileStream(path, FileMode.Open, FileAccess.Read);

            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                f.CopyTo(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            var resultStream = new MemoryStream(fileBytes);
            return resultStream;

        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Could not find file '{path}': {ex.Message}");
            return null;
        }
    }
}
