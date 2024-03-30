using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using CN.Desktop.Display.Helpers;
using CN.Desktop.Display.Providers;
using CN.Models.Roles;

namespace CN.Desktop.Display.Viewmodels;
public class RolesItemViewModel
{
    public event Action SomethingChanged = delegate { };

    private readonly AllowedSender sender;
    private readonly Guid channelId;
    public RolesItemViewModel(Guid channelId, AllowedSender sender)
    {
        this.sender = sender;
        this.channelId = channelId;
    }

    public string? Name => this.sender.Name;
    public string? Id => this.sender.Id.ToString();

    public ICommand DeleteCommand => new CommandHandler(async () =>
    {
        try
        {
            if (!await DialogHelper.ShowConfirmationMessage($"Deseja remover o usuário \"{this.sender.Name}\" deste canal?", "Roles"))
                return;

            await RolesManager.Remove(this.channelId, this.sender.Id);
            SomethingChanged.Invoke();
        }
        catch (Exception e)
        {
            await DialogHelper.ShowMessage(e.Message, "Roles");
        }
    });
}
