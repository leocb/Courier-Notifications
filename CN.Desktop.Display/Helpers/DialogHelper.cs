using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CN.Desktop.Display.Views;
using MaterialDesignThemes.Wpf;

namespace CN.Desktop.Display.Helpers;
public static class DialogHelper
{
    public static async Task ShowMessage(string message)
    {
        var view = new MessageDialog(message);
        _ = await DialogHost.Show(view);
    }
}
