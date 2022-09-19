using Nest;
using Reservation.ElasticSearch.Document;

namespace Reservation.ElasticSearch.Configure;

public class ReservationElasticIndex
{
    public static async Task MakeSureExistsAsync(IElasticClient elasticClient)
    {
        var result = await ExistsAsync(elasticClient);

        if (!result.Exists)
        {
            await CreateReservationIndexAsync(elasticClient);
        }
    }

    private static async Task<string> CreateReservationIndexAsync(IElasticClient elasticClient)
    {
        var result = await elasticClient.Indices.CreateAsync(ElasticConfiguration.IndexConfig.ReservationEventIndex,
            c => c
                .Map(m => m
                    .AutoMap<ReservationDocument>()
                )
        );
        return result.Index;
    }

    private static async Task<ExistsResponse> ExistsAsync(IElasticClient elasticClient)
    {
        var result = await elasticClient.Indices.ExistsAsync(
            new IndexExistsRequest(Indices.Index(ElasticConfiguration.IndexConfig.ReservationEventIndex)));
        return result;
    }
}