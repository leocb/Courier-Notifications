using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;

using CN.Desktop.Display.Viewmodels;
using CN.Models.Messages;

using Microsoft.Win32;

using WebSocket4Net;

namespace CN.Desktop.Display.Providers;

public static class ConnectionManager
{
    public static ConnectionViewmodel Viewmodel { get; set; } = new ConnectionViewmodel();
    private static readonly ObservableCollection<WebSocket> webSockets = [];

    private static readonly Guid DisplayId;
    private static readonly string RegistryKeyPath = @"HKEY_CURRENT_USER\Software\Courier Notifications";
    private static readonly string RegistryValueName = @"id";

    static ConnectionManager()
    {
        if (Registry.GetValue(RegistryKeyPath, RegistryValueName, null) is Guid id)
        {
            DisplayId = id;
        }
        else
        {
            DisplayId = Guid.NewGuid();
            Registry.SetValue(RegistryKeyPath, RegistryValueName, DisplayId);
        }

        Viewmodel.Status = Models.ConnectionStatus.None;
    }

    public static void OpenAllChannels()
    {

        if (string.IsNullOrWhiteSpace(Properties.Settings.Default.ServerUrl) ||
            string.IsNullOrWhiteSpace(Properties.Settings.Default.ServerChannels))
        {
            Viewmodel.Status = Models.ConnectionStatus.Disconnected;
            return;
        }

        if (webSockets.Where(ws => ws.State == WebSocketState.Open).ToArray().Length > 0)
        {
            CloseAllChannels();
        }

        Viewmodel.Status = Models.ConnectionStatus.Connecting;

        Properties.Settings.Default.Save();

        // only allow Alphanumeric uppercase chars, - and , to be evaluated
        string[] Channels = Regex.Replace(Properties.Settings.Default.ServerChannels.ToUpper(), @"([^A-Z0-9,\-])+", "").Split(",");

        foreach (string Channel in Channels)
        {
            Open(Properties.Settings.Default.ServerUrl + Channel);
        }

        if (Channels.Length != webSockets.Where(ws => ws.State == WebSocketState.Open).ToArray().Length)
        {
            CloseAllChannels();
            MessageDisplayManager.AddInternalMessage(new Message()
            { Text = $"Error: Some channels could not be connected.", Date = DateTime.Now, From = DisplayId });
            Viewmodel.Status = Models.ConnectionStatus.Error;
            return;
        }

        Viewmodel.Status = Models.ConnectionStatus.Connected;

    }

    public static void CloseAllChannels()
    {
        Viewmodel.Status = Models.ConnectionStatus.Disconnecting;

        foreach (WebSocket? ws in webSockets.Where(ws => ws.State == WebSocketState.Open).ToArray())
        {
            Close(ws);
        }

        webSockets.Clear();
        Viewmodel.Status = Models.ConnectionStatus.Disconnected;
    }

    private static void Open(string serverUri)
    {
        try
        {
            WebSocket ws = new(serverUri);
            ws.Security.AllowCertificateChainErrors = true;
            ws.Security.AllowUnstrustedCertificate = true;
            ws.Security.AllowNameMismatchCertificate = true;
            ws.AutoSendPingInterval = 50;
            ws.EnableAutoSendPing = true;
            ws.Closed += OnWsClose;
            ws.Opened += OnWsOpen;
            ws.Error += OnWsError;
            ws.MessageReceived += OnWsMessage;
            ws.Open();

            while (ws.State == WebSocketState.Connecting)
            {
                _ = Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                { }));
            }

            webSockets.Add(ws);
        }
        catch (Exception ex)
        {
            MessageDisplayManager.AddInternalMessage(new Message()
            { Text = $"Error: {ex.Message}", Date = DateTime.Now, From = DisplayId });
        }
    }
    private static void Close(WebSocket ws)
    {
        ws.Close("Connection terminated by the user");
        while (ws.State == WebSocketState.Closing)
        {
            _ = Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
            { }));
        }
    }

    public static string CreateRandomChannelID() => Guid.NewGuid().ToString();

    private static void OnWsMessage(object? sender, MessageReceivedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.Message))
            return;

        // Internal server message
        try
        {
            Message? message = JsonSerializer.Deserialize<Message>(e.Message, Models.Options.JsonSerializer);
            if (message is null)
            {
                throw new Exception("Failed to parse");
            }

            MessageDisplayManager.AddDisplayMessage(message);
        }
        catch (Exception ex)
        {
            MessageDisplayManager.AddInternalMessage(new Message()
            { Text = $"Error: {ex.Message}\nOriginal: {e.Message}", Date = DateTime.Now, From = DisplayId },
            MessageStatus.Failed);
        }
    }

    private static void OnWsOpen(object? sender, EventArgs e)
    {
        if (sender == null)
            return;

        PropertyInfo itemProperty = typeof(WebSocket).GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).Single(pi => pi.Name == "TargetUri");
        Uri? wsTarget = itemProperty.GetValue(sender, null) as Uri;

        MessageDisplayManager.AddInternalMessage(new Message()
        { Text = $"Connected to {wsTarget?.ToString()}", Date = DateTime.Now, From = DisplayId },
        MessageStatus.Info);
    }

    private static void OnWsClose(object? sender, EventArgs e)
    {
        if (sender == null)
            return;

        PropertyInfo itemProperty = typeof(WebSocket).GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).Single(pi => pi.Name == "TargetUri");
        Uri? wsTarget = itemProperty.GetValue(sender, null) as Uri;

        MessageDisplayManager.AddInternalMessage(new Message()
        { Text = $"Disconnected from {wsTarget?.ToString()}", Date = DateTime.Now, From = DisplayId },
        MessageStatus.Info);
    }

    private static void OnWsError(object? sender, SuperSocket.ClientEngine.ErrorEventArgs e) => MessageDisplayManager.AddInternalMessage(new Message()
    { Text = $"Error: {e.Exception.Message}", Date = DateTime.Now, From = DisplayId });

}
