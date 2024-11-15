using Cultivation.Dto.FertilizerLand;
using Cultivation.Shared.Enum;
using Microsoft.AspNetCore.Http;

namespace Cultivation.Repository.File;

public interface IFileRepo
{
    (FormFile file, MemoryStream stream) ExportToExcel(ExportType type, string fileName, List<string> filters, List<ExportToExcelDto> dtos);
}
