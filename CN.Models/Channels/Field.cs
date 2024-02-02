namespace CN.Models.Channels;

public class Field
{
    public string Name { get; set; } = "";
    public string Value { get; set; } = "";
    public bool Required { get; set; } = false;
    public string RegexValidation { get; set; } = @"^.*$";
    public string RegexForAlternate { get; set; } = @"empty";
    public string TextBeforeValue { get; set; } = "";
    public string TextAfterValue { get; set; } = "";
    public string TextBeforeValueAlternate { get; set; } = "";
    public string TextAfterValueAlternate { get; set; } = "";
}