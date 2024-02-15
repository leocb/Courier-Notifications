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
using CN.Models.Roles;

namespace CN.Desktop.Display.Providers;
public static class RolesManager
{
    public static event ManagerStatusHandler OnStatusChanged = delegate { };

    private static readonly HttpClient client = new();
    private static readonly string baseUrl = Properties.Settings.Default.ServerUrl;

    public static Dictionary<Guid, ObservableCollection<AllowedSender>> Roles { get; } = [];

    static RolesManager()
    {
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("User-Agent", "Courier Notifications Display");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("ownerId", ConnectionManager.OwnerId.ToString());
    }

    public static async Task<List<AllowedSender>> Get(Guid channelId)
    {
        try
        {
            OnStatusChanged.Invoke(ManagerStatus.Busy);

            using HttpRequestMessage request = new(
            HttpMethod.Get,
            $"{baseUrl}/api/channel/role?channelId={channelId}");

            using HttpResponseMessage response = (await client.SendAsync(request)).EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<AllowedSender>>(Options.JsonSerializer) ?? [];
        }
        finally
        {
            OnStatusChanged.Invoke(ManagerStatus.Available);
        }
    }

    public static async Task GetAll()
    {
        try
        {
            OnStatusChanged.Invoke(ManagerStatus.Busy);

            List<Guid> channelIds = ChannelManager.Channels.Select(c => c.Id).ToList();

            if (channelIds.Count == 0)
                return;

            using HttpRequestMessage request = new(
                HttpMethod.Get,
                $"{baseUrl}/api/channel/role/bulk")
            {
                Content = new StringContent(
                JsonSerializer.Serialize(channelIds, Options.JsonSerializer),
                null,
                "application/json")
            };

            using HttpResponseMessage response = (await client.SendAsync(request)).EnsureSuccessStatusCode();
            List<ChannelRoles> roleList = new(await response.Content.ReadFromJsonAsync<List<ChannelRoles>>(Options.JsonSerializer) ?? []);

            Roles.Clear();
            roleList.ForEach(role =>
            {
                Roles.Add(role.Id, new(role.AllowedSenders));
            });
        }
        finally
        {
            OnStatusChanged.Invoke(ManagerStatus.Available);
        }
    }

    public static async Task Add(Guid channelId, AllowedSender sender)
    {
        try
        {
            OnStatusChanged.Invoke(ManagerStatus.Busy);

            using HttpRequestMessage request = new(
            HttpMethod.Post,
            $"{baseUrl}/api/channel/role/sender?channelId={channelId}")
            {
                Content = new StringContent(
            JsonSerializer.Serialize(sender, Options.JsonSerializer),
            null,
            "application/json")
            };

            using HttpResponseMessage response = (await client.SendAsync(request)).EnsureSuccessStatusCode();

            if (Roles.TryGetValue(channelId, out ObservableCollection<AllowedSender>? roles))
            {
                roles.Add(sender);
            }
        }
        finally
        {
            OnStatusChanged.Invoke(ManagerStatus.Available);
        }
    }

    public static async Task Remove(Guid channelId, Guid senderId)
    {
        try
        {
            OnStatusChanged.Invoke(ManagerStatus.Busy);

            using HttpRequestMessage request = new(
            HttpMethod.Delete,
            $"{baseUrl}/api/channel/role/sender?channelId={channelId}?senderId={senderId}");

            using HttpResponseMessage response = (await client.SendAsync(request)).EnsureSuccessStatusCode();

            if (Roles.TryGetValue(channelId, out ObservableCollection<AllowedSender>? roles))
            {
                _ = roles.Remove(roles.First(roles => roles.Id == senderId));
            }
        }
        finally
        {
            OnStatusChanged.Invoke(ManagerStatus.Available);
        }
    }
}
