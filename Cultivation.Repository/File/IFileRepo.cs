using Cultivation.Shared.Enum;
using Microsoft.AspNetCore.Http;

namespace Cultivation.Repository.File;

public interface IFileRepo<T> where T : class
{
    (FormFile file, MemoryStream stream) ExportToExcel(ExportType type, string fileName, List<string> filters, List<T> dtos);
}
