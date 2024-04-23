using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CN.Models.Channels
{
    public static class FieldExtensions
    {
        public static void ClearFields(this List<Field> fields)
        {
            fields.ForEach(f =>{f.Value = "";});
        }

        public static bool ValidateField(this Field field)
        {
            if (string.IsNullOrEmpty(field.RegexValidation)) return true;
            Regex regexValidation = new(field.RegexValidation);
            return regexValidation.IsMatch(field.Value);
        }

        public static bool IsUsingAlternate(this Field field)
        {
            if (string.IsNullOrEmpty(field.RegexForAlternate)) return false;
            Regex regexAlternate = new(field.RegexForAlternate);
            return regexAlternate.IsMatch(field.Value);
        }

        public static string GetFieldFinalText(this Field field)
        {
            if (!field.ValidateField() && string.IsNullOrEmpty(field.Value)) return "";

            if (field.IsUsingAlternate())
            {
                return $"{field.TextBeforeValueAlternate}{field.Value}{field.TextAfterValueAlternate}";
            }
            else
            {
                return $"{field.TextBeforeValue}{field.Value}{field.TextAfterValue}";
            }
        }

        public static string GetChannelFinalText(this List<Field> allFields)
        {
            StringBuilder sb = new StringBuilder();
            allFields.ForEach(f => { sb.Append(f.GetFieldFinalText()); });
            return sb.ToString();
        }
        public static bool IsAllValid(this List<Field> allFields)
        {
            foreach (var field in allFields)
            {
                if (!field.ValidateField())
                    return false;
            }
            return true;
        }
    }
}
