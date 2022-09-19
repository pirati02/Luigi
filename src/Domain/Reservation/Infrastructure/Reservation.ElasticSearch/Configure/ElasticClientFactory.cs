using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Nest;
using Reservation.ElasticSearch.Document;

namespace Reservation.ElasticSearch.Configure;

public class ElasticClientFactory
{
    public static ElasticClient BuildElasticClient(IConfiguration configuration)
    {
        var url = configuration["ElasticConfiguration:Uri"];
        var pool = new SingleNodeConnectionPool(new Uri(url));
        var settings = new ConnectionSettings(pool)
            .DefaultMappingFor<ReservationDocument>(a =>
                a.IndexName(ElasticConfiguration.IndexConfig.ReservationEventIndex));
        var client = new ElasticClient(settings);

        ReservationElasticIndex.MakeSureExistsAsync(client).GetAwaiter().GetResult();
        return client;
    }
}