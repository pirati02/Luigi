using Nest;
using Reservation.ElasticSearch;
using Reservation.ElasticSearch.Configure;
using Reservation.ElasticSearch.Document;

namespace Reservation.Api.Service;

public interface IActiveReservationFinder
{
    Task<bool> CheckDraftSessionExists(int row, int column, Guid sessionId, CancellationToken ct);
    Task<IHit<ReservationDocument>> GetReservationDocumentAsync(Guid sessionId, Guid initiatorUserId,
        Guid reservationId, CancellationToken ct);
}

public class ActiveReservationFinder : IActiveReservationFinder
{
    private readonly IElasticClient _elasticClient;

    public ActiveReservationFinder(
        IElasticClient elasticClient
    )
    {
        _elasticClient = elasticClient;
    }

    public async Task<bool> CheckDraftSessionExists(int row, int column, Guid sessionId, CancellationToken ct)
    {
        var result = await _elasticClient.SearchAsync<ReservationDocument>(s => s
                .Index(ElasticConfiguration.IndexConfig.ReservationEventIndex)
                .From(0)
                .Size(1)
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            mu => mu
                                .Term(t => t
                                    .Field(f => f.SessionId)
                                    .Value(sessionId)),
                            mu => mu
                                .Term(t => t
                                    .Field(f => f.Row)
                                    .Value(row)),
                            mu => mu
                                .Term(t => t
                                    .Field(f => f.Column)
                                    .Value(column))
                        )
                    )
                )
            , ct);
        return result.Documents.Any();
    }
 
    public async Task<IHit<ReservationDocument>> GetReservationDocumentAsync(Guid sessionId, Guid initiatorUserId,
        Guid reservationId, CancellationToken ct)
    {
        var result = await _elasticClient.SearchAsync<ReservationDocument>(s => s
                .Index(ElasticConfiguration.IndexConfig.ReservationEventIndex)
                .From(0)
                .Size(1)
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            mu => mu
                                .Term(t => t
                                    .Field(f => f.SessionId)
                                    .Value(sessionId)),
                            mu => mu
                                .Term(t => t
                                    .Field(f => f.InitiatorUserId)
                                    .Value(initiatorUserId)),
                            mu => mu
                                .Term(t => t
                                    .Field(f => f.ReservationId)
                                    .Value(reservationId))
                        )
                    )
                )
            , ct);
        return result.Hits.FirstOrDefault()!;
    }
}