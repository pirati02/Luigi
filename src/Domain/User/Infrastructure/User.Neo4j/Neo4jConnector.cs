using Microsoft.Extensions.Configuration;
using Neo4jClient;

namespace User.Neo4j;

public class Neo4jConnector
{
    public static BoltGraphClient Connect(IConfiguration configuration)
    {
        var (url, userName, password) = UnwrapConnectionString(configuration);

        var client = new BoltGraphClient(new Uri(url), userName, password);
        client.ConnectAsync().GetAwaiter().GetResult();
        return client;
    }

    private static (string url, string userName, string password) UnwrapConnectionString(IConfiguration configuration)
    {
        var url = configuration["Neo4j:uri"]; 
        var userName = configuration["Neo4j:username"]; 
        var password = configuration["Neo4j:password"]; 
        return (url, userName, password);
    }
}