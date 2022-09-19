using Nest;
using User.Infrastructure.Document;

namespace User.Infrastructure.Elastic;

public class UserElasticIndex
{
    public static async Task MakeSureExistsAsync(IElasticClient elasticClient)
    {
        var result = await ExistsAsync(elasticClient);

        if (!result.Exists)
        {
            await CreateUserIndexAsync(elasticClient);
        }
    }

    private static async Task<string> CreateUserIndexAsync(IElasticClient elasticClient)
    {
        var result = await elasticClient.Indices.CreateAsync(ElasticConfiguration.IndexConfig.UsersIndex,
            c => c
                .Map(m => m
                    .AutoMap<UserDocument>()
                )
        );
        return result.Index;
    }

    private static async Task<ExistsResponse> ExistsAsync(IElasticClient elasticClient)
    {
        var result = await elasticClient.Indices.ExistsAsync(
            new IndexExistsRequest(Indices.Index(ElasticConfiguration.IndexConfig.UsersIndex)));
        return result;
    }
}