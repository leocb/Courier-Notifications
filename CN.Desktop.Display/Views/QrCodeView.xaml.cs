using System;
using System.Windows;

using CN.Desktop.Display.Viewmodels;

namespace CN.Desktop.Display.Views;

/// <summary>
/// Interaction logic for QrCodeView.xaml
/// </summary>
public partial class QrCodeView : Window
{
    public QrCodeView(Guid channelId)
    {
        RoleQrCodeItemViewModel vm = new(channelId);
        this.DataContext = vm;
        vm.CloseRequest += Close;
        InitializeComponent();
    }
}
