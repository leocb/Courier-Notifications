using System.Threading.Tasks;

using CN.Desktop.Display.Views;

using MaterialDesignThemes.Wpf;

namespace CN.Desktop.Display.Helpers;
public static class DialogHelper
{
    public static async Task ShowMessage(string message, string dgHostId)
    {
        MessageDialog view = new(message, false);
        _ = await DialogHost.Show(view, dgHostId);
    }
    public static async Task<bool> ShowConfirmationMessage(string message, string dgHostId)
    {
        MessageDialog view = new(message, true);
        return (bool)(await DialogHost.Show(view, dgHostId) ?? false);
    }
}
