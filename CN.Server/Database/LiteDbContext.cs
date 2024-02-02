using LiteDB;

using Microsoft.Extensions.Options;

namespace CN.Server.Database;

public class LiteDbContext(IOptions<LiteDbOptions> options)
{
    private static readonly string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LiteDb", "CN.db");
    public LiteDatabase Database { get; } = new LiteDatabase(path);//options.Value.DatabaseLocation);
}