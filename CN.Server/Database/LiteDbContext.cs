using LiteDB;

using Microsoft.Extensions.Options;

namespace CN.Server.Database;

public class LiteDbContext
{
    public static string ChannelDbName { get; } = "Channels.db";
    public LiteDatabase DatabaseInstance { get; }

    public LiteDbContext()
    {
        BsonMapper.Global.EmptyStringToNull = false;
        BsonMapper.Global.TrimWhitespace = false;

        string dbBasePath = OperatingSystem.IsWindows()
            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db")
            : "/database";

        if (!Directory.Exists(dbBasePath))
            _ = Directory.CreateDirectory(dbBasePath);

        this.DatabaseInstance = InitDatabase(dbBasePath, ChannelDbName);
    }
    private static LiteDatabase InitDatabase(string dbBasePath, string dbName)
    {
        string dbFilePath = Path.Combine(dbBasePath, dbName);

        LiteDatabase db = new(dbFilePath)
        {
            UserVersion = 1
        };
        return db;
    }
}