using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using CN.Models;
using CN.Models.Exceptions;
using CN.Models.Messages;

using WebSocket4Net;

namespace CN.Desktop.Display.Providers;

public enum ConnectionStatus
{
    None,
    Connected,
    Disconnected,
    Connecting,
    Disconnecting,
    Error
}

public static class SocketManager
{
    public delegate void StatusUpdateHandler(ConnectionStatus status);
    public static event StatusUpdateHandler OnStatusChanged = delegate { };
    private static readonly List<WebSocket> webSockets = [];

    static SocketManager()
    {
        OnStatusChanged.Invoke(ConnectionStatus.None);
    }

    public static async Task OpenAll()
    {

        if (string.IsNullOrWhiteSpace(Properties.Settings.Default.ServerUrl))
        {
            OnStatusChanged.Invoke(ConnectionStatus.Disconnected);
            return;
        }

        if (webSockets.Where(ws => ws.State == WebSocketState.Open).ToArray().Length > 0)
            await CloseAllChannels();

        OnStatusChanged.Invoke(ConnectionStatus.Connecting);
        try
        {
            await ChannelManager.LoadChannelsFromServer();
        }
        catch (Exception ex)
        {
            SystemMessage.Error($"Error: {ex.Message}");
            OnStatusChanged.Invoke(ConnectionStatus.Error);
            return;
        }

        foreach (Guid channelId in ChannelManager.Channels.Select(c => c.Id))
        {
            await Open(channelId);
        }

        if (ChannelManager.Channels.Count != webSockets.Count(ws => ws.State == WebSocketState.Open))
        {
            await CloseAllChannels();
            OnStatusChanged.Invoke(ConnectionStatus.Error);
            SystemMessage.Error($"Error: Some channels could not be connected.");
            return;
        }

        OnStatusChanged.Invoke(ConnectionStatus.Connected);

    }

    public static async Task CloseAllChannels()
    {
        OnStatusChanged.Invoke(ConnectionStatus.Disconnecting);

        foreach (WebSocket? ws in webSockets.Where(ws => ws.State == WebSocketState.Open))
        {
            await Close(ws);
        }

        webSockets.Clear();
        OnStatusChanged.Invoke(ConnectionStatus.Disconnected);
    }

    private static async Task Open(Guid channelId)
    {
        try
        {
            string uri = $"{Properties.Settings.Default.ServerUrl.Replace("http", "ws")}/api/ws/connect?channelId={channelId}";

            WebSocket ws = new(uri)
            {
                Security = {
                    AllowCertificateChainErrors = true,
                    AllowUnstrustedCertificate = true,
                    AllowNameMismatchCertificate = true,
                },
                AutoSendPingInterval = 50,
                EnableAutoSendPing = true
            };

            ws.Error += OnWsError;
            ws.MessageReceived += OnWsMessage;

            _ = await ws.OpenAsync();

            while (ws.State == WebSocketState.Connecting)
            {
                _ = Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate
                { }));
            }

            webSockets.Add(ws);
        }
        catch (Exception ex)
        {
            OnStatusChanged.Invoke(ConnectionStatus.Error);
            SystemMessage.Error($"Error: {ex.Message}");
        }
    }

    private static async Task Close(WebSocket ws)
    {
        try
        {
            _ = await ws.CloseAsync();
        }
        catch (Exception ex)
        {
            OnStatusChanged.Invoke(ConnectionStatus.Error);
            SystemMessage.Error($"Error: {ex.Message}. Socket abandoned.");
        }
    }

    private static void OnWsMessage(object? sender, MessageReceivedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.Message))
            return;

        try
        {
            Message? message = JsonSerializer.Deserialize<Message>(e.Message, Options.JsonSerializer) ?? throw new CourierException("Failed to parse");
            MessageQueue.Add(message);
        }
        catch (Exception ex)
        {
            SystemMessage.Error($"Error: {ex.Message}\nOriginal: {e.Message}");
        }
    }

    private static void OnWsError(object? sender, SuperSocket.ClientEngine.ErrorEventArgs e)
    {
        string message = $"WS Error: {e.Exception.Message}";
        OnStatusChanged.Invoke(ConnectionStatus.Error);
        SystemMessage.Error(message);
    }
}
