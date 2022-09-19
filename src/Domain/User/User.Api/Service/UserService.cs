using Nest;
using User.Domain;
using User.Infrastructure;
using User.Infrastructure.Document;

namespace User.Api.Service;

public interface IUserService
{
    Task<IHit<UserDocument>> FindBy(Email email, CancellationToken ct);

    Task<string> RegisterAsync(
        string firstName,
        string lastName,
        string mobile,
        string email,
        int? countryCode,
        CancellationToken ct
    );
}

public class UserService : IUserService
{
    private readonly IElasticClient _elasticClient;

    public UserService(IElasticClient elasticClient)
    {
        _elasticClient = elasticClient;
    }

    public async Task<IHit<UserDocument>> FindBy(Email email, CancellationToken ct)
    {
        var document = await _elasticClient.SearchAsync<UserDocument>(s => s
                .Index(ElasticConfiguration.IndexConfig.UsersIndex)
                .From(0)
                .Size(1)
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            mu => mu
                                .Term(t => t
                                    .Field(f => f.Email)
                                    .Value(email.Value))
                        )
                    )
                )
            , ct);

        return document.Hits.FirstOrDefault()!;
    }

    public async Task<string> RegisterAsync(
        string firstName,
        string lastName,
        string mobile,
        string email,
        int? countryCode,
        CancellationToken ct)
    {
        var document = new UserDocument(
            firstName,
            lastName,
            email,
            mobile,
            countryCode
        );
        var result = await _elasticClient.IndexAsync(
            document,
            a => a.Index(ElasticConfiguration.IndexConfig.UsersIndex),
            ct
        );
        return result.Id;
    }
}