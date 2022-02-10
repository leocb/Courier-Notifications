using CN.Desktop.Display.Viewmodels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WebSocket4Net;
using System.Xml.Serialization;


namespace CN.Desktop.Display.Providers
{
    public static class ConnectionManager
    {
        public static ConnectionViewmodel Viewmodel { get; set; } = new ConnectionViewmodel();
        private static Random random = new Random();

        static ConnectionManager()
        {
            Viewmodel.Status = "Initializing Connection";
        }

        public static void Open()
        {
            var ws = new WebSocket(Properties.Settings.Default.ServerUrl);
            ws.Security.AllowCertificateChainErrors = true;
            ws.Security.AllowUnstrustedCertificate = true;
            ws.Security.AllowNameMismatchCertificate = true;
            ws.Opened += OnWsOpen;
            ws.Error += OnWsError;
            ws.Closed += OnWsClose;
            ws.MessageReceived += OnWsMessage;
            ws.AutoSendPingInterval = 5;
            ws.EnableAutoSendPing = true;
            ws.Open();
        }


        public static string GetRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static void OnWsMessage(object? sender, MessageReceivedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Message)) return;
            if (!e.Message.Contains("xml"))
            {
                MessageDisplayManager.AddInfoMessage(new Models.Messages.Message()
                { Text = e.Message, Date = DateTime.Now, FromName = "Websocket" },
                Models.Messages.MessageStatus.Info);
                return;
            }

            try
            {
                var xml = new XmlSerializer(typeof(Models.Messages.Message));
                Models.Messages.Message msg;

                using (TextReader sr = new StringReader(e.Message))
                {
                    MessageDisplayManager.AddMessage((Models.Messages.Message)xml.Deserialize(sr));
                }

            }
            catch (Exception ex)
            {
                MessageDisplayManager.AddInfoMessage(new Models.Messages.Message()
                { Text = $"Error: {ex.Message}\nValue: {e.Message}", Date = DateTime.Now, FromName = "Message Error" });
            }
        }

        private static void OnWsOpen(object? sender, EventArgs e)
        {
            var message = new Models.Messages.Message()
            { FromName = "Leo", Date = DateTime.Now, Text = "Mensagem Marota." };

            var xml = new XmlSerializer(typeof(Models.Messages.Message));
            string xmlText;

            using (var sw = new StringWriter())
            {
                xml.Serialize(sw, message);
                xmlText = sw.ToString();
            }

            (sender as WebSocket)?.Send(xmlText);

            MessageDisplayManager.AddInfoMessage(new Models.Messages.Message()
            { Text = $"Connected!", Date = DateTime.Now, FromName = "Websocket" },
            Models.Messages.MessageStatus.Info);
        }

        private static void OnWsClose(object? sender, EventArgs e)
        {
            MessageDisplayManager.AddInfoMessage(new Models.Messages.Message()
            { Text = $"Connection Closed", Date = DateTime.Now, FromName = "Websocket" },
            Models.Messages.MessageStatus.Info);
        }

        private static void OnWsError(object? sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            MessageDisplayManager.AddInfoMessage(new Models.Messages.Message()
            { Text = $"Error: {e.Exception.Message}", Date = DateTime.Now, FromName = "Websocket" });
        }

    }
}
