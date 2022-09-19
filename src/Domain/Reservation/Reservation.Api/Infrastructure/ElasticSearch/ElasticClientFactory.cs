using Elasticsearch.Net;
using Nest;
using Reservation.Api.Feature;

namespace Reservation.Api.Infrastructure.ElasticSearch;

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