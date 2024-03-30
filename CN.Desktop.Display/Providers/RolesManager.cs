using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

using CN.Models;
using CN.Models.Exceptions;
using CN.Models.Roles;

namespace CN.Desktop.Display.Providers;
public static class RolesManager
{
    public static event ManagerStatusHandler OnStatusChanged = delegate { };

    private static readonly HttpClient client = new();
    private static readonly string baseUrl = Properties.Settings.Default.ServerUrl;

    public static Dictionary<Guid, ObservableCollection<AllowedSender>> ChannelInfo { get; } = [];
    public static Dictionary<Guid, (string UserName, string ChannelName)> UserInfo { get; } = [];

    static RolesManager()
    {
        client.Timeout = TimeSpan.FromSeconds(5);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("User-Agent", "Courier Notifications Display");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("ownerId", ConnectionManager.OwnerId.ToString());

        UserInfo.Add(ConnectionManager.OwnerId, ("Info", "Display"));
    }

    public static async Task GetAllRolesFromServer()
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
            List<ChannelRoles> channelRoleList = new(await response.Content.ReadFromJsonAsync<List<ChannelRoles>>(Options.JsonSerializer) ?? []);
            UpdateChannelRoleList(channelRoleList);
        }
        finally
        {
            OnStatusChanged.Invoke(ManagerStatus.Available);
        }
    }

    private static void UpdateChannelRoleList(List<ChannelRoles> channelRoleList)
    {
        foreach (ChannelRoles channelRole in channelRoleList)
        {
            // Update channel info
            if (ChannelInfo.TryGetValue(channelRole.Id, out ObservableCollection<AllowedSender>? localRoles))
            {
                localRoles.Clear();
                foreach (AllowedSender role in channelRole.AllowedSenders)
                {
                    localRoles.Add(role);
                }
            }
            else
            {
                ChannelInfo.Add(channelRole.Id, new(channelRole.AllowedSenders));
            }

            // Update user info
            foreach (AllowedSender role in channelRole.AllowedSenders)
            {
                if (UserInfo.TryGetValue(role.Id, out (string UserName, string ChannelName) userInfo))
                {
                    userInfo.UserName = role.Name;
                    userInfo.ChannelName = ChannelManager.Channels.First(x => x.Id == channelRole.Id).Name;
                }
                else
                {
                    UserInfo.Add(role.Id, (role.Name, ChannelManager.Channels.First(x => x.Id == channelRole.Id).Name));
                }
            }
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

            UserInfo.Add(sender.Id, (sender.Name, ChannelManager.Channels.First(x => x.Id == channelId).Name));
            if (ChannelInfo.TryGetValue(channelId, out ObservableCollection<AllowedSender>? roles))
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
            $"{baseUrl}/api/channel/role/sender?channelId={channelId}&senderId={senderId}");

            using HttpResponseMessage response = (await client.SendAsync(request)).EnsureSuccessStatusCode();

            _ = UserInfo.Remove(senderId);
            if (ChannelInfo.TryGetValue(channelId, out ObservableCollection<AllowedSender>? roles))
            {
                _ = roles.Remove(roles.First(roles => roles.Id == senderId));
            }
        }
        finally
        {
            OnStatusChanged.Invoke(ManagerStatus.Available);
        }
    }

    public static void RemoveFromLocalList(Guid channelId) => ChannelInfo.Remove(channelId);

    public static async Task GetServerId()
    {
        try
        {
            OnStatusChanged.Invoke(ManagerStatus.Busy);
            using HttpRequestMessage request = new(
            HttpMethod.Get,
            $"{baseUrl}/api/server/id");

            using HttpResponseMessage response = (await client.SendAsync(request)).EnsureSuccessStatusCode();
            Guid serverId = await response.Content.ReadFromJsonAsync<Guid?>(Options.JsonSerializer) ?? throw new CourierException("Unknown Server");
            _ = UserInfo.TryAdd(serverId, ("Info", "Servidor"));
        }
        finally
        {
            OnStatusChanged.Invoke(ManagerStatus.Available);
        }
    }
}
