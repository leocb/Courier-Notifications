using CN.Desktop.Display.Viewmodels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;
using WebSocket4Net;

namespace CN.Desktop.Display.Providers
{
    public static class ConnectionManager
    {
        public static ConnectionViewmodel Viewmodel { get; set; } = new ConnectionViewmodel();
        private static Random random = new Random();
        private static ObservableCollection<WebSocket> webSockets = new ObservableCollection<WebSocket>();

        static ConnectionManager()
        {
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
            var Channels = Regex.Replace(Properties.Settings.Default.ServerChannels.ToUpper(), @"([^A-Z0-9,\-])+", "").Split(",");

            foreach (var Channel in Channels)
            {
                Open(Properties.Settings.Default.ServerUrl + Channel);
            }

            if (Channels.Length != webSockets.Where(ws => ws.State == WebSocketState.Open).ToArray().Length)
            {
                CloseAllChannels();
                MessageDisplayManager.AddInternalMessage(new Models.Messages.Message()
                { Text = $"Error: Some channels could not be connected.", Date = DateTime.Now, FromName = "Connection manager" });
                Viewmodel.Status = Models.ConnectionStatus.Error;
                return;
            }

            Viewmodel.Status = Models.ConnectionStatus.Connected;

        }

        public static void CloseAllChannels()
        {
            Viewmodel.Status = Models.ConnectionStatus.Disconnecting;

            foreach (var ws in webSockets.Where(ws => ws.State == WebSocketState.Open).ToArray())
            {
                Close(ws);
            };

            webSockets.Clear();
            Viewmodel.Status = Models.ConnectionStatus.Disconnected;
        }

        private static void Open(string serverUri)
        {
            try
            {
                var ws = new WebSocket(serverUri);
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
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
                }

                webSockets.Add(ws);
            }
            catch (Exception ex)
            {
                MessageDisplayManager.AddInternalMessage(new Models.Messages.Message()
                { Text = $"Error: {ex.Message}", Date = DateTime.Now, FromName = "Connection manager" });
            }
        }
        private static void Close(WebSocket ws)
        {
            ws.Close("Connection terminated by the user");
            while (ws.State == WebSocketState.Closing)
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
            }
        }


        public static void SendMessageToAllChannels(string message, string fromName)
        {
            var messageModel = new Models.Messages.Message()
            { FromName = fromName, Date = DateTime.Now, Text = message };

            var xml = new XmlSerializer(typeof(Models.Messages.Message));
            string xmlText;

            using (var sw = new StringWriter())
            {
                xml.Serialize(sw, messageModel);
                xmlText = sw.ToString();
            }

            foreach (var ws in webSockets)
            {
                ws.Send(xmlText);
            }
        }

        public static void SendInternalMessage(string message)
        {
            foreach (var ws in webSockets)
            {
                ws.Send(message);
            }
        }

        public static string CreateRandomChannelID()
        {
            return Guid.NewGuid().ToString();
        }

        private static void OnWsMessage(object? sender, MessageReceivedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Message)) return;

            // Internal server message
            if (!e.Message.Contains("xml"))
            {
                MessageDisplayManager.AddInternalMessage(new Models.Messages.Message()
                { Text = e.Message, Date = DateTime.Now, FromName = "Websocket Server" },
                e.Message.ToUpper().Contains("ERROR") ? Models.Messages.MessageStatus.Failed : Models.Messages.MessageStatus.Info);
                return;
            }

            // xml means a properly formatted message, show on screen
            try
            {
                var xml = new XmlSerializer(typeof(Models.Messages.Message));

                using (TextReader sr = new StringReader(e.Message))
                {
                    MessageDisplayManager.AddDisplayMessage((Models.Messages.Message)xml.Deserialize(sr));
                }

            }
            catch (Exception ex)
            {
                MessageDisplayManager.AddInternalMessage(new Models.Messages.Message()
                { Text = $"Error: {ex.Message}\nValue: {e.Message}", Date = DateTime.Now, FromName = "Message Error" });
            }
        }

        private static void OnWsOpen(object? sender, EventArgs e)
        {
            if (sender == null) return;

            PropertyInfo itemProperty = typeof(WebSocket).GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).Single(pi => pi.Name == "TargetUri");
            var wsTarget = itemProperty.GetValue(sender, null) as Uri;

            MessageDisplayManager.AddInternalMessage(new Models.Messages.Message()
            { Text = $"Connected to {wsTarget?.ToString()}", Date = DateTime.Now, FromName = "Websocket" },
            Models.Messages.MessageStatus.Info);
        }

        private static void OnWsClose(object? sender, EventArgs e)
        {
            if (sender == null) return;

            PropertyInfo itemProperty = typeof(WebSocket).GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).Single(pi => pi.Name == "TargetUri");
            var wsTarget = itemProperty.GetValue(sender, null) as Uri;

            MessageDisplayManager.AddInternalMessage(new Models.Messages.Message()
            { Text = $"Disconnected from {wsTarget?.ToString()}", Date = DateTime.Now, FromName = "Websocket" },
            Models.Messages.MessageStatus.Info);
        }

        private static void OnWsError(object? sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            MessageDisplayManager.AddInternalMessage(new Models.Messages.Message()
            { Text = $"Error: {e.Exception.Message}", Date = DateTime.Now, FromName = "Websocket" });
        }

    }
}
