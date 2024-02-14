using CN.Models.Channels;
using CN.Models.Exceptions;
using CN.Models.Roles;
using CN.Server.Database;

namespace CN.Server.Providers;

public class ChannelDataProvider
{
    private readonly LiteDbContext context;
    private readonly LiteDB.ILiteCollection<Channel?> channelsDb;
    private readonly LiteDB.ILiteCollection<ChannelRoles?> channelRolesDb;

    public ChannelDataProvider(LiteDbContext context)
    {
        this.context = context;
        this.channelsDb = this.context.ChannelDatabase.GetCollection<Channel?>("Channels");
        this.channelRolesDb = this.context.ChannelDatabase.GetCollection<ChannelRoles?>("ChannelRoles");
    }

    public async Task<Guid> CreateChannel(Guid ownerId, Channel data)
    {
        return await Task.Run(() =>
        {
            Guid newChannelId = Guid.NewGuid();
            data.Id = newChannelId;

            ChannelRoles roles = new()
            {
                Owner = ownerId,
                AllowedSenders = [new(ownerId, "Owner")]
            };

            this.channelRolesDb.Insert(newChannelId, roles);
            this.channelsDb.Insert(newChannelId, data);

            return newChannelId;

        }).ConfigureAwait(false);
    }

    public async Task UpdateChannel(Guid channelId, Guid ownerId, Channel data)
    {
        await Task.Run(() =>
        {
            GetRoles(channelId).ValidateOwner(ownerId);

            _ = this.channelsDb.Update(channelId, data);

        }).ConfigureAwait(false);
    }

    public async Task DeleteChannel(Guid channelId, Guid ownerId)
    {
        await Task.Run(() =>
        {
            GetRoles(channelId).ValidateOwner(ownerId);

            _ = this.channelRolesDb.Delete(ownerId);
            _ = this.channelsDb.Delete(channelId);

        }).ConfigureAwait(false);
    }

    public async Task<Channel> GetChannel(Guid channelId)
    {
        return await Task.Run(() =>
        {
            Channel? channel = this.channelsDb.FindById(channelId);

            if (channel is null)
                throw new CourierException("channel not found.");

            return channel;

        }).ConfigureAwait(false);
    }

    public async Task<List<Channel>> GetChannelBulk(List<Guid> channelList)
    {
        return await Task.Run(() =>
        {
            List<Channel> channelData = [];
            foreach (Guid channelId in channelList)
            {
                Channel? channel = this.channelsDb.FindById(channelId);

                if (channel is null)
                    continue;

                channelData.Add(channel);
            }

            return channelData;

        }).ConfigureAwait(false);

    }

    public async Task<ChannelRoles> GetChannelRoles(Guid channelId, Guid ownerId)
    {
        return await Task.Run(() =>
        {
            ChannelRoles roles = GetRoles(channelId);
            roles.ValidateOwner(ownerId);

            return roles;
        }).ConfigureAwait(false);
    }

    public async Task VerifyAllowedSender(Guid channelId, Guid senderId)
    {
        await Task.Run(() =>
        {
            GetRoles(channelId).ValidateAllowedSender(senderId);
        }).ConfigureAwait(false);
    }

    public async Task<ChannelRoles> AddSenderToChannelRoles(Guid channelId, Guid ownerId, AllowedSender newSender)
    {
        return await Task.Run(() =>
        {
            ChannelRoles roles = GetRoles(channelId);
            roles.ValidateOwner(ownerId);

            roles.AllowedSenders.Add(newSender);
            _ = this.channelRolesDb.Update(channelId, roles);

            return roles;
        }).ConfigureAwait(false);
    }

    public async Task<ChannelRoles> RemoveSenderFromChannelRoles(Guid channelId, Guid ownerId, Guid removedSenderId)
    {
        return await Task.Run(() =>
        {
            ChannelRoles roles = GetRoles(channelId);
            roles.ValidateOwner(ownerId);

            _ = roles.AllowedSenders.RemoveAll(x => x.Id == removedSenderId);
            _ = this.channelRolesDb.Update(channelId, roles);

            return roles;
        }).ConfigureAwait(false);
    }

    private ChannelRoles GetRoles(Guid channelId)
    {
        ChannelRoles? roles = this.channelRolesDb.FindById(channelId);

        if (roles is null)
            throw new CourierException("channel not found");

        return roles;
    }
}
