using System.Text.RegularExpressions;

namespace CN.Models.Centrals;

public class Field
{
    public string Name { get; set; } = string.Empty;
    public int Size { get; set; }
    public bool Required { get; set; }
    public bool OnlyNumbers { get; set; }
    public Regex? SecondaryAnswear { get; set; }
    public string? TextBeforeValue { get; set; }
    public string? TextAfterValue { get; set; }
    public string? TextAfterValueSecondary { get; set; }
}
