using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

using CN.Models;
using CN.Models.Channels;
using CN.Models.Exceptions;

namespace CN.Desktop.Display.Managers;
internal static class ChannelManager
{

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
        List<string> channelIds = Properties.Settings.Default.ServerChannels?.Cast<string>().ToList() ?? [];

        if (channelIds.Count == 0)
            return;

        HttpRequestMessage request = new(
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

    internal static async Task<Channel> Get(Guid channelId)
    {
        Channel? channel;
        HttpRequestMessage request = new(
            HttpMethod.Get,
            $"{baseUrl}/api/channel?channelId={channelId}");

        using HttpResponseMessage response = (await client.SendAsync(request)).EnsureSuccessStatusCode();
        channel = await response.Content.ReadFromJsonAsync<Channel>(Options.JsonSerializer);

        if (channel is null)
            throw new CourierException("Channel data failed to load");

        return channel;
    }

    internal static async Task Create(Channel channel)
    {
        HttpRequestMessage request = new(
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
        Properties.Settings.Default.ServerChannels.Add(channel.Id.ToString());
        Properties.Settings.Default.Save();
    }

    internal static async Task Update(Channel upChannel)
    {
        HttpRequestMessage request = new(
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

    internal static async Task Delete(Guid channelId)
    {
        HttpRequestMessage request = new(
            HttpMethod.Delete,
            $"{baseUrl}/api/channel?channelId={channelId}");

        using HttpResponseMessage response = (await client.SendAsync(request)).EnsureSuccessStatusCode();
    }
}
