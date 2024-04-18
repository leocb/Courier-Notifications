using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;

using Blazored.LocalStorage;

using CN.Models;
using CN.Models.Channels;
using CN.Models.Messages;

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
            await this.localStorage.GetItemAsync<List<ChannelUser>>(this.localStorageKey) ?? []
        );
    }

    public async Task GetAllChannelsFromServer()
    {
        try
        {
            OnStatusChanged.Invoke(ManagerStatus.Busy);

            List<string> channelIds = this.KnownChannels.Select(c => c.Channel.ToString()).ToList();

            if (channelIds.Count <= 0)
                return;

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

            this.Channels.Clear();
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
        Guid senderId = this.KnownChannels.Find(c => c.Channel == channel.Id)?.User
            ?? throw new ArgumentException("Invalid Channel.");

        Message message = new()
        {
            Date = DateTime.Now,
            Status = MessageStatus.Queued,
            From = senderId,
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

    public async Task Add(Guid channelId, Guid userId)
    {
        if (this.localStorage is null)
            return;

        // Update channel allowed user ID if it exists
        ChannelUser? c = this.KnownChannels.Find(c => c.Channel == channelId);
        if (c is not null)
        {
            c.User = userId;
        }
        // Create a new entry if it doesn't
        else
        {
            this.KnownChannels.Add(new() { Channel = channelId, User = userId });
        }
        // save to local storage
        await this.localStorage.SetItemAsync(this.localStorageKey, this.KnownChannels);
    }

    public async Task RemoveAsync(Guid channelId)
    {
        if (this.localStorage is null)
            return;

        ChannelUser? c = this.KnownChannels.Find(c => c.Channel == channelId);
        if (c is null)
            return;

        _ = this.KnownChannels.Remove(c);
        await this.localStorage.SetItemAsync(this.localStorageKey, this.KnownChannels);
    }
}
