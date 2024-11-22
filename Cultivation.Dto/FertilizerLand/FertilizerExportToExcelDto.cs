using System.ComponentModel;

namespace Cultivation.Dto.FertilizerLand;

public class FertilizerExportToExcelDto
{
    [DisplayName("Oygulama Tarihi")]
    public string Date { get; set; }

    [DisplayName("Gübre Miktarı")]
    public string Quantity { get; set; } //kg
    [DisplayName("Sınıf")]
    public string Type { get; set; }

    //public LandDto Land { get; set; }

    //fertilizer
    [DisplayName("NPK")]
    public string NPK { get; set; }
    [DisplayName("Bilimsel Ad")]
    public string Title { get; set; }
    [DisplayName("Tecari Ad")]
    public string PublicTitle { get; set; }
    [DisplayName("Tanım")]
    public string Description { get; set; }
}