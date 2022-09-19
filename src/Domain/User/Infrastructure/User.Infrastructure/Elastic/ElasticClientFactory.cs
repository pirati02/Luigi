using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Nest;
using User.Infrastructure.Document;

namespace User.Infrastructure.Elastic;

public class ElasticClientFactory
{
    public static ElasticClient BuildElasticClient(IConfiguration configuration)
    {
        var url = configuration["ElasticConfiguration:Uri"];
        var pool = new SingleNodeConnectionPool(new Uri(url));
        var settings = new ConnectionSettings(pool)
            .DefaultMappingFor<UserDocument>(a =>
                a.IndexName(ElasticConfiguration.IndexConfig.UsersIndex));
        var client = new ElasticClient(settings);

        UserElasticIndex.MakeSureExistsAsync(client).GetAwaiter().GetResult();
        return client;
    }
}