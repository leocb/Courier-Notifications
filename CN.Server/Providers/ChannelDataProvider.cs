using CN.Models.Servers;
using CN.Server.Database;
using CN.Server.Exceptions;

namespace CN.Server.Providers;

public class ChannelDataProvider
{
    private readonly LiteDbContext context;
    private readonly LiteDB.ILiteCollection<Channel?> channels;
    private readonly LiteDB.ILiteCollection<OwnerData?> owners;

    public ChannelDataProvider(LiteDbContext context)
    {
        this.context = context;
        this.channels = this.context.Database.GetCollection<Channel?>("Channels");
        this.owners = this.context.Database.GetCollection<OwnerData?>("ChannelsOwner");
    }

    public async Task<Guid> CreateChannel(Guid ownerId, Channel data)
    {
        return await Task.Run(() =>
        {
            Guid newChannelId = Guid.NewGuid();
            data.Id = newChannelId;
            OwnerData owner = new() { Owner = ownerId };
            this.owners.Insert(newChannelId, owner);
            this.channels.Insert(newChannelId, data);
            return newChannelId;

        }).ConfigureAwait(false);
    }

    public async Task UpdateChannel(Guid ownerId, Guid channelId, Channel data)
    {
        await Task.Run(() =>
        {
            OwnerData? owner = this.owners.FindById(channelId);

            if (owner is null || owner.Owner != ownerId)
                throw new CourierException("you know what you did.");

            _ = this.channels.Update(channelId, data);

        }).ConfigureAwait(false);
    }

    public async Task DeleteChannel(Guid ownerId, Guid channelId)
    {
        await Task.Run(() =>
        {
            OwnerData? owner = this.owners.FindById(channelId);

            if (owner is null || owner.Owner != ownerId)
                throw new CourierException("you know what you did.");

            _ = this.owners.Delete(ownerId);
            _ = this.channels.Delete(channelId);

        }).ConfigureAwait(false);
    }

    public async Task<Channel> GetChannel(Guid channelId)
    {
        return await Task.Run(() =>
        {
            Channel? channel = this.channels.FindById(channelId);

            if (channel is null)
                throw new CourierException("channel not found.");

            return channel;

        }).ConfigureAwait(false);

    }

    public async Task<List<Channel>> GetChannelBulk(List<Guid> channelList)
    {
        return await Task.Run(() =>
        {
            List<Channel> returnData = [];
            foreach (Guid channelId in channelList)
            {
                Channel? channel = this.channels.FindById(channelId);
                if (channel is null)
                {
                    continue;
                }

                returnData.Add(channel);
            }

            return returnData;

        }).ConfigureAwait(false);

    }
}
