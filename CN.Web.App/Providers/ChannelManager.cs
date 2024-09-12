using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;

using CN.Models;
using CN.Models.Channels;
using CN.Models.Messages;

using Microsoft.JSInterop;

namespace CN.Web.App.Providers;

public class ChannelManager(HttpClient client)
{
    public event ManagerStatusHandler OnStatusChanged = delegate { };

    private readonly string localStorageKey = "KnownChannels";

    private readonly HttpClient client = client;

    private ILocalStorageService? localStorage;

    public ObservableCollection<Channel> Channels { get; } = [];
    public List<ChannelUser> KnownChannels { get; } = [];

    public async Task SetLocalStorage(ILocalStorageService localStorage)
    {
        this.localStorage = localStorage;
        this.KnownChannels.Clear();
        this.KnownChannels.AddRange(
            this.localStorage.GetItem<List<ChannelUser>>(this.localStorageKey) ?? []
        );
    }

    public async Task GetAllChannelsFromServer()
    {
        try
        {
            OnStatusChanged.Invoke(ManagerStatus.Busy);
            this.Channels.Clear();

            List<string> channelIds = this.KnownChannels.Select(c => c.Channel.ToString()).ToList();

            if (channelIds.Count <= 0)
            {
                OnStatusChanged.Invoke(ManagerStatus.Available);
                return;
            }

            using HttpRequestMessage request = new(
                HttpMethod.Post,
                $"api/channel/bulk")
            {
                Content = new StringContent(
                JsonSerializer.Serialize(channelIds, Options.JsonSerializer),
                null,
                "application/json")
            };

            using HttpResponseMessage response = (await this.client.SendAsync(request)).EnsureSuccessStatusCode();

            List<Channel>? loadedChannels = await response.Content.ReadFromJsonAsync<List<Channel>>(Options.JsonSerializer) ?? [];

            foreach (Channel channel in loadedChannels)
            {
                this.Channels.Add(channel);
            }
        }
        finally
        {
            OnStatusChanged.Invoke(ManagerStatus.Available);
        }
    }

    public async Task Send(Channel channel)
    {
        Guid authId = this.KnownChannels.Find(c => c.Channel == channel.Id)?.Auth
            ?? throw new ArgumentException("Invalid Channel.");

        Message message = new()
        {
            Date = DateTime.Now,
            Status = MessageStatus.Queued,
            From = authId,
            Text = channel.Fields.GetChannelFinalText()
        };

        using HttpRequestMessage request = new(
                HttpMethod.Post,
                $"api/message/send?channel={channel.Id}")
        {
            Content = new StringContent(
                JsonSerializer.Serialize(message, Options.JsonSerializer),
                null,
                "application/json")
        };

        using HttpResponseMessage response = await this.client.SendAsync(request);
        _ = response.EnsureSuccessStatusCode();
    }

    public async Task AddAsync(Guid channelId, Guid authId)
    {
        if (this.localStorage is null)
            return;

        // Update channel allowed user ID if it exists
        ChannelUser? c = this.KnownChannels.Find(c => c.Channel == channelId);
        if (c is not null)
        {
            c.Auth = authId;
        }

        // Create a new entry if it doesn't
        else
        {
            this.KnownChannels.Add(new() { Channel = channelId, Auth = authId });
        }

        // save to local storage
        this.localStorage.SetItem(this.localStorageKey, this.KnownChannels);
    }

    public async Task RemoveAsync(Guid channelId)
    {
        if (this.localStorage is null)
            return;

        ChannelUser? c = this.KnownChannels.Find(c => c.Channel == channelId);
        if (c is null)
            return;

        _ = this.KnownChannels.Remove(c);
        this.localStorage.SetItem(this.localStorageKey, this.KnownChannels);
    }
}
