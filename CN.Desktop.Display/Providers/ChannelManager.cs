using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using CN.Models;
using CN.Models.Channels;

namespace CN.Desktop.Display.Providers;

public static class ChannelManager
{
    public static event ManagerStatusHandler OnStatusChanged = delegate { };

    private static readonly HttpClient client = new();
    private static readonly string baseUrl = Properties.Settings.Default.ServerUrl;

    public static ObservableCollection<Channel> Channels { get; } = [];

    static ChannelManager()
    {
        client.Timeout = TimeSpan.FromSeconds(5);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("User-Agent", "Courier Notifications Display");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("ownerId", ConnectionManager.OwnerId.ToString());
    }

    public static async Task GetAllChannelsFromServer()
    {
        try
        {
            OnStatusChanged.Invoke(ManagerStatus.Busy);

            List<string> channelIds = Properties.Settings.Default.ServerChannels?.Cast<string>().ToList() ?? [];

            if (channelIds.Count == 0)
                return;

            using HttpRequestMessage request = new(
                HttpMethod.Post,
                $"{baseUrl}/api/channel/bulk")
            {
                Content = new StringContent(
                JsonSerializer.Serialize(channelIds, Options.JsonSerializer),
                null,
                "application/json")
            };

            using HttpResponseMessage response = (await client.SendAsync(request)).EnsureSuccessStatusCode();

            List<Channel>? loadedChannels = await response.Content.ReadFromJsonAsync<List<Channel>>(Options.JsonSerializer) ?? [];

            Channels.Clear();
            foreach (Channel channel in loadedChannels)
            {
                Channels.Add(channel);
            }

            await RolesManager.GetAllRolesFromServer();
        }
        finally
        {
            OnStatusChanged.Invoke(ManagerStatus.Available);
        }
    }

    public static async Task Create(Channel channel)
    {
        try
        {
            OnStatusChanged.Invoke(ManagerStatus.Busy);

            using HttpRequestMessage request = new(
            HttpMethod.Post,
            $"{baseUrl}/api/channel")
            {
                Content = new StringContent(
                JsonSerializer.Serialize(channel, Options.JsonSerializer),
                null,
                "application/json")
            };

            using HttpResponseMessage response = (await client.SendAsync(request)).EnsureSuccessStatusCode();
            channel.Id = await response.Content.ReadFromJsonAsync<Guid>(Options.JsonSerializer);
            _ = Properties.Settings.Default.ServerChannels.Add(channel.Id.ToString());
            Properties.Settings.Default.Save();
            Channels.Add(channel);
            await RolesManager.GetAllRolesFromServer();
        }
        finally
        {
            OnStatusChanged.Invoke(ManagerStatus.Available);
        }
    }

    public static async Task Update(Channel upChannel)
    {
        try
        {
            OnStatusChanged.Invoke(ManagerStatus.Busy);

            using HttpRequestMessage request = new(
            HttpMethod.Put,
            $"{baseUrl}/api/channel?channelId={upChannel.Id}")
            {
                Content = new StringContent(
                JsonSerializer.Serialize(upChannel, Options.JsonSerializer),
                Encoding.UTF8,
                "application/json")
            };

            using HttpResponseMessage response = (await client.SendAsync(request)).EnsureSuccessStatusCode();
        }
        finally
        {
            OnStatusChanged.Invoke(ManagerStatus.Available);
        }
    }

    public static async Task Delete(Guid channelId)
    {
        try
        {
            OnStatusChanged.Invoke(ManagerStatus.Busy);

            using HttpRequestMessage request = new(
            HttpMethod.Delete,
            $"{baseUrl}/api/channel?channelId={channelId}");

            using HttpResponseMessage response = (await client.SendAsync(request)).EnsureSuccessStatusCode();
            _ = Channels.Remove(Channels.First(c => c.Id == channelId));
            RolesManager.RemoveFromLocalList(channelId);
        }
        finally
        {
            OnStatusChanged.Invoke(ManagerStatus.Available);
        }
    }
}
