using System.ComponentModel;

namespace Cultivation.Dto.InsecticideLand;

public class InsecticideExportToExcelDto
{
    [DisplayName("Oygulama Tarihi")]
    public string Date { get; set; }

    [DisplayName("Ilaç Miktarı")]
    public string Quantity { get; set; } //kg

    [DisplayName("Ilaç Litre")]
    public string Liter { get; set; }

    [DisplayName("Sınıf")]
    public string Type { get; set; }

    //public LandDto Land { get; set; }

    //insecticide

    [DisplayName("Bilimsel Ad")]
    public string Title { get; set; }
    [DisplayName("Tecari Ad")]
    public string PublicTitle { get; set; }
    [DisplayName("Tanım")]
    public string Description { get; set; }
}