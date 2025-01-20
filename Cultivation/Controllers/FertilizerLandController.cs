using Cultivation.Dto.FertilizerLand;
using Cultivation.Repository.FertilizerLand;
using Microsoft.AspNetCore.Mvc;

namespace Cultivation.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class FertilizerLandController : ControllerBase
{
    private readonly IFertilizerLandRepo fertilizerLandRepo;

    public FertilizerLandController(IFertilizerLandRepo fertilizerLandRepo)
    {
        this.fertilizerLandRepo = fertilizerLandRepo;
    }
    [HttpPost]
    public async Task<IActionResult> Add(FertilizerLandFormDto dto)
    {
        await fertilizerLandRepo.AddAsync(dto);
        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(long? landId, DateTime? from, DateTime? to, int pageSize = 10, int pageNum = 0)
    {
        var result = await fertilizerLandRepo.GetAllAsync(landId, from, to, pageSize, pageNum);
        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetLandsWhichNotUsedInDay(DateTime? date)
    {
        var result = await fertilizerLandRepo.GetLandsWhichNotUsedInDayAsync(date);
        return Ok(result);
    }
    //[HttpGet]
    //public async Task<IActionResult> GetFertilizersLand(long landId, DateTime? from, DateTime? to, int pageSize = 10, int pageNum = 0)
    //{
    //    var result = await fertilizerLandRepo.GetFertilizersLandAsync(landId, from, to, pageSize, pageNum);
    //    return Ok(result);
    //}
    [HttpGet]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await fertilizerLandRepo.GetByIdAsync(id);
        return Ok(result);
    }
    [HttpPost]
    public async Task<IActionResult> ExportExcel(long landId, DateTime? from, DateTime? to, string fileName = "ExcelFile")
    {
        //Console.WriteLine("EXCEL PASS HERE ON CONTROLLER");
        ////var (file, stream) = await fertilizerLandRepo.ExportExcelFileAsync(landId, from, to);
        //var result = await fertilizerLandRepo.GetFertilizersLandAsync(landId, from, to);
        //var toExport = result.Data.Select(x => new ExportToExcelDto
        //{
        //    Type = x.Type.ToString(),
        //    Date = x.Date.ToShortDateString(),
        //    Quantity = x.Quantity.ToString(),
        //    NPK = x.Fertilizer.NPK,
        //    Title = x.Fertilizer.Title,
        //    Description = x.Fertilizer.Description,
        //    PublicTitle = x.Fertilizer.PublicTitle,
        //}).ToList();
        ////List<string> filtersPropertiesNames = new();
        ////Type filterPropertiesType = typeof(HcpPatientFilterDto);
        ////PropertyInfo[] filterProperties = filterPropertiesType.GetProperties();
        ////foreach (var item in filterProperties)
        ////{
        ////    var valueOfFilterProp = (bool)item.GetValue(filterDto);
        ////    if (valueOfFilterProp)
        ////        filtersPropertiesNames.Add(item.Name);
        ////}

        //var excel = ExportToExcel(ExportType.LandFertilizers, fileName, new(), /*filtersPropertiesNames,*/ toExport);

        var excel = await fertilizerLandRepo.ExportExcelAsync(landId, from, to, fileName);
        return File(excel.stream, excel.file.ContentType, excel.file.FileName);
    }
    [HttpPost]
    public async Task<IActionResult> Update(long id, UpdateFertilizerLandDto dto)
    {
        await fertilizerLandRepo.UpdateAsync(id, dto);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> Remove(long id)
    {
        await fertilizerLandRepo.RemoveAsync(id);
        return Ok();
    }
    #region MixLand
    [HttpPost]
    public async Task<IActionResult> AddMixLand(long mixId, long cuttingLandId)
    {
        await fertilizerLandRepo.AddMixLandAsync(mixId, cuttingLandId);
        return Ok();
    }
    [HttpPost]
    public async Task<IActionResult> AddMixLands(long mixId, List<long> cuttingLandIds)
    {
        await fertilizerLandRepo.AddMixLandsAsync(mixId, cuttingLandIds);
        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> GetMixLands(string landTitle, string fertilizerMixTitle, string mixTitle, DateTime? mixedDate)
    {
        var result = await fertilizerLandRepo.GetMixLandsAsync(landTitle, mixTitle, mixedDate);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveMixLand(long mixLandId)
    {
        await fertilizerLandRepo.RemoveMixLandAsync(mixLandId);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveMixLands(List<long> mixLandIds)
    {
        await fertilizerLandRepo.RemoveMixLandsAsync(mixLandIds);
        return Ok();
    }

    #endregion
    //    private static (FormFile file, MemoryStream stream) ExportToExcel(ExportType type, string fileName, List<string> filters, List<FertilizerExportToExcelDto> dtos)
    //    {
    //        IWorkbook workbook = new XSSFWorkbook();
    //        ISheet sheet = workbook.CreateSheet("Gübre Oygulama");

    //        // Create cell styles for title.
    //        var headerStyle = workbook.CreateCellStyle();
    //        IFont font = workbook.CreateFont();
    //        font.IsBold = true;
    //        headerStyle.WrapText = true;
    //        headerStyle.FillForegroundColor = IndexedColors.LightBlue.Index;
    //        headerStyle.FillPattern = FillPattern.SolidForeground;
    //        headerStyle.Alignment = HorizontalAlignment.Center;
    //        headerStyle.VerticalAlignment = VerticalAlignment.Center;
    //        headerStyle.SetFont(font);

    //        // Set the value of a cell
    //        switch (type)
    //        {
    //            case ExportType.LandFertilizers:
    //                Type exportDtoType = typeof(InsecticideExportToExcelDto);
    //                PropertyInfo[] exportDtoProp = exportDtoType.GetProperties();
    //                IRow titleRow = sheet.CreateRow(0);
    //                int columnNumber = 0;
    //                foreach (PropertyInfo property in exportDtoProp)
    //                {
    //                    //if (filters.Contains(property.Name))
    //                    //{
    //                    string title = ((DisplayNameAttribute)Attribute.GetCustomAttribute(property, typeof(DisplayNameAttribute))).DisplayName;
    //                    titleRow.CreateCell(columnNumber).SetCellValue(title);
    //                    titleRow.Cells[columnNumber].CellStyle = headerStyle;
    //                    ++columnNumber;
    //                    //}
    //                }
    //                int row = 1;
    //                foreach (var dto in dtos)
    //                {
    //                    int columnInfoNumber = 0;
    //                    IRow infoRow = sheet.CreateRow(row);
    //                    foreach (PropertyInfo property in exportDtoProp)
    //                    {
    //                        //if (filters.Contains(property.Name))
    //                        //{
    //                        var valueOfPatientProp = property.GetValue(dto);
    //                        var typeOfPatientProp = property.PropertyType;
    //                        if (valueOfPatientProp != null && typeOfPatientProp.IsGenericType)
    //                        {
    //                            string f = "";
    //                            List<string> listValues = Convert.ChangeType(valueOfPatientProp, typeof(List<string>)) as List<string>;
    //                            foreach (var item in listValues)
    //                            {
    //                                f = $"{item}\n";
    //                            }
    //                            infoRow.CreateCell(columnInfoNumber).SetCellValue(f);
    //                        }
    //                        else if (valueOfPatientProp != null)
    //                            infoRow.CreateCell(columnInfoNumber).SetCellValue(valueOfPatientProp.ToString());
    //                        ++columnInfoNumber;
    //                        //}
    //                    }
    //                    row++;
    //                }
    //                break;
    //        }
    //        MemoryStream stream = new();
    //        workbook.Write(stream);
    //        var fileBytes = stream.ToArray();
    //        var memoryStream = new MemoryStream(fileBytes);
    //        //var fileName = ${fileName+ ".xlsx"};
    //        var formFile = new FormFile(memoryStream, 0, memoryStream.Length, null, $"{fileName}.xlsx")

    //        {
    //            Headers = new HeaderDictionary(),
    //            ContentType = "application/vnd.ms-excel"
    //        };
    //        return (formFile, memoryStream);
    //    }
}
