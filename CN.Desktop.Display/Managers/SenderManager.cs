using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

using CN.Models;
using CN.Models.Roles;

namespace CN.Desktop.Display.Managers;
internal static class SenderManager
{

    private static readonly HttpClient client = new();
    private static readonly string baseUrl = Properties.Settings.Default.ServerUrl;

    static SenderManager()
    {
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("User-Agent", "Courier Notifications Display");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.DefaultRequestHeaders.Add("ownerId", ConnectionManager.OwnerId.ToString());
    }

    internal static async Task<List<AllowedSender>> Get(Guid channelId)
    {
        HttpRequestMessage request = new(
            HttpMethod.Get,
            $"{baseUrl}/api/channel/role?channelId={channelId}");

        using HttpResponseMessage response = (await client.SendAsync(request)).EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<AllowedSender>>(Options.JsonSerializer) ?? [];
    }

    internal static async Task Add(Guid channelId, AllowedSender sender)
    {
        HttpRequestMessage request = new(
            HttpMethod.Post,
            $"{baseUrl}/api/channel/role/sender?channelId={channelId}")
        {
            Content = new StringContent(
            JsonSerializer.Serialize(sender, Options.JsonSerializer),
            null,
            "application/json")
        };

        using HttpResponseMessage response = (await client.SendAsync(request)).EnsureSuccessStatusCode();
    }

    internal static async Task Remove(Guid channelId, Guid senderId)
    {
        HttpRequestMessage request = new(
            HttpMethod.Delete,
            $"{baseUrl}/api/channel/role/sender?channelId={channelId}?senderId={senderId}");

        using HttpResponseMessage response = (await client.SendAsync(request)).EnsureSuccessStatusCode();
    }
}
