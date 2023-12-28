using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CN.Desktop.Display.Converters;

[ValueConversion(typeof(System.Windows.Media.Color), typeof(System.Drawing.Color))]
public class AnimationSidesMarginConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => new Thickness(-System.Convert.ToDouble(value) * 5000, 10, System.Convert.ToDouble(value), 10);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => -((Thickness)value).Left;
}

