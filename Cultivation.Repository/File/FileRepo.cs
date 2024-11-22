using Cultivation.Database.Context;
using Cultivation.Shared.Enum;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.ComponentModel;
using System.Reflection;

namespace Cultivation.Repository.File;
public class FileRepo<T> : IFileRepo<T> where T : class
{
    private readonly CultivationDbContext context;

    public FileRepo(CultivationDbContext context)
    {
        this.context = context;
    }
    public (FormFile file, MemoryStream stream) ExportToExcel(ExportType type, string fileName, List<string> filters, List<T> dtos)
    {
        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = workbook.CreateSheet("Gübre Oygulama");

        // Create cell styles for title.
        var headerStyle = workbook.CreateCellStyle();
        IFont font = workbook.CreateFont();
        font.IsBold = true;
        headerStyle.WrapText = true;
        font.FontHeightInPoints = 11;
        font.FontName = "Calibri";

        headerStyle.BorderBottom = BorderStyle.Thin;
        headerStyle.BorderTop = BorderStyle.Thin;
        headerStyle.BorderLeft = BorderStyle.Thin;
        headerStyle.BorderRight = BorderStyle.Thin;

        headerStyle.FillForegroundColor = IndexedColors.Green.Index;
        headerStyle.FillPattern = FillPattern.SolidForeground;
        headerStyle.Alignment = HorizontalAlignment.Center;
        headerStyle.VerticalAlignment = VerticalAlignment.Center;
        headerStyle.SetFont(font);

        // Set the value of a cell
        switch (type)
        {
            case ExportType.LandFertilizers:
                Type exportDtoType = typeof(T);
                PropertyInfo[] exportDtoProp = exportDtoType.GetProperties();
                IRow titleRow = sheet.CreateRow(0);
                int columnNumber = 0;
                foreach (PropertyInfo property in exportDtoProp)
                {
                    //if (filters.Contains(property.Name))
                    //{
                    string title = ((DisplayNameAttribute)Attribute.GetCustomAttribute(property, typeof(DisplayNameAttribute))).DisplayName;
                    titleRow.CreateCell(columnNumber).SetCellValue(title);
                    titleRow.Cells[columnNumber].CellStyle = headerStyle;
                    ++columnNumber;
                    //}
                }
                int row = 1;
                foreach (var dto in dtos)
                {
                    int columnInfoNumber = 0;
                    IRow infoRow = sheet.CreateRow(row);
                    foreach (PropertyInfo property in exportDtoProp)
                    {
                        //if (filters.Contains(property.Name))
                        //{
                        var valueOfPatientProp = property.GetValue(dto);
                        var typeOfPatientProp = property.PropertyType;
                        if (valueOfPatientProp != null && typeOfPatientProp.IsGenericType)
                        {
                            string f = "";
                            List<string> listValues = Convert.ChangeType(valueOfPatientProp, typeof(List<string>)) as List<string>;
                            foreach (var item in listValues)
                            {
                                f = $"{item}\n";
                            }
                            infoRow.CreateCell(columnInfoNumber).SetCellValue(f);
                        }
                        else if (valueOfPatientProp != null)
                            infoRow.CreateCell(columnInfoNumber).SetCellValue(valueOfPatientProp.ToString());
                        ++columnInfoNumber;
                        //}
                    }
                    row++;
                }
                break;
        }
        MemoryStream stream = new();
        workbook.Write(stream);
        var fileBytes = stream.ToArray();
        var memoryStream = new MemoryStream(fileBytes);
        //var fileName = ${fileName+ ".xlsx"};
        var formFile = new FormFile(memoryStream, 0, memoryStream.Length, null, $"{fileName}.xlsx")

        {
            Headers = new HeaderDictionary(),
            ContentType = "application/vnd.ms-excel"
        };
        return (formFile, memoryStream);
    }
}
