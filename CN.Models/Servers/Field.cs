using System.Text.RegularExpressions;

namespace CN.Models.Servers;

public class Field
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool Required { get; set; }
    public Regex Validation { get; set; } = new(@"^.*$", RegexOptions.Compiled);
    public bool IsValid => Validation.IsMatch(Value);
    public Regex RegexForAlternate { get; set; } = new(@"empty", RegexOptions.Compiled);
    public string? TextBeforeValue { get; set; }
    public string? TextAfterValue { get; set; }
    public string? TextBeforeValueAlternate { get; set; }
    public string? TextAfterValueAlternate { get; set; }
}
