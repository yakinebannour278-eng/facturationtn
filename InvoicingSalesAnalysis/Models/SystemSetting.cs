using System.ComponentModel.DataAnnotations;

namespace InvoicingSalesAnalysis.Models;

public static class SystemSettingKeys
{
    public const string TaxStampAmount = "TaxStampAmount";
}

public class SystemSetting
{
    [Key]
    [MaxLength(128)]
    public string Key { get; set; } = string.Empty;

    [Required]
    [MaxLength(128)]
    public string Value { get; set; } = "1.000";
}
