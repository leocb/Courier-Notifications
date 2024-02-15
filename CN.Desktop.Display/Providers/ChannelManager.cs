using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

using CN.Desktop.Display.Managers;
using CN.Models;
using CN.Models.Channels;
using CN.Models.Exceptions;

namespace CN.Desktop.Display.Providers;

public static class ChannelManager
{
    public static event ManagerStatusHandler OnStatusChanged = delegate { };

    private static readonly HttpClient client = new();
    private static readonly string baseUrl = Properties.Settings.Default.ServerUrl;

    public static ObservableCollection<Channel> Channels { get; private set; } = [];

    static ChannelManager()
    {
        client.Timeout = TimeSpan.FromSeconds(5);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("User-Agent", "Courier Notifications Display");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("ownerId", ConnectionManager.OwnerId.ToString());
    }

    public static async Task LoadChannelsFromServer()
    {
        try
        {
            OnStatusChanged.Invoke(ManagerStatus.Busy);

            List<string> channelIds = Properties.Settings.Default.ServerChannels?.Cast<string>().ToList() ?? [];

            if (channelIds.Count == 0)
                return;

            using HttpRequestMessage request = new(
                HttpMethod.Get,
                $"{baseUrl}/api/channel/bulk")
            {
                Content = new StringContent(
                JsonSerializer.Serialize(channelIds, Options.JsonSerializer),
                null,
                "application/json")
            };

            using HttpResponseMessage response = (await client.SendAsync(request)).EnsureSuccessStatusCode();
            Channels = new(await response.Content.ReadFromJsonAsync<List<Channel>>(Options.JsonSerializer) ?? []);
        }
        finally
        {
            OnStatusChanged.Invoke(ManagerStatus.Available);
        }
    }

    public static async Task<Channel> Get(Guid channelId)
    {
        try
        {
            OnStatusChanged.Invoke(ManagerStatus.Busy);

            Channel? channel;
            using HttpRequestMessage request = new(
                HttpMethod.Get,
                $"{baseUrl}/api/channel?channelId={channelId}");

            using HttpResponseMessage response = (await client.SendAsync(request)).EnsureSuccessStatusCode();
            channel = await response.Content.ReadFromJsonAsync<Channel>(Options.JsonSerializer);

            if (channel is null)
                throw new CourierException("Channel data failed to load");

            return channel;
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
            null,
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
        }
        finally
        {
            OnStatusChanged.Invoke(ManagerStatus.Available);
        }
    }
}
