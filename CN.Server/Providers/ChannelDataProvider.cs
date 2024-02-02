
using CN.Models.Servers;

namespace CN.Server.Providers;

public class ChannelDataProvider
{
    public async Task<Guid> CreateChannel(Guid ownerId, Channel data)
    {
        return Guid.NewGuid();
    }

    public async Task UpdateChannel(Guid ownerId, Guid channelId, Channel data)
    {

    }

    public async Task DeleteChannel(Guid ownerId, Guid channelId) {
    }

    public async Task<Channel> GetChannel(Guid channelId)
    {
        return new Channel();
    }

    public async Task<List<Channel>> GetChannelBulk(List<Guid> channelId)
    {
        return new List<Channel>() { new Channel() };
    }
}
