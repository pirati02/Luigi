using Neo4jClient;

namespace User.Api.Service;

public interface IUserService
{
    Task<Neo4j.User?> FindByAsync(string email, CancellationToken ct);
    Task<Neo4j.User> LoginAsync(string email, string password, CancellationToken ct);

    Task RegisterAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        string mobile,
        int? countryCode
    );
}

public class UserService : IUserService
{
    private readonly IGraphClient _graphClient;
    private readonly IPasswordGenerator _passwordGenerator;

    public UserService(IGraphClient graphClient, IPasswordGenerator passwordGenerator)
    {
        _graphClient = graphClient;
        _passwordGenerator = passwordGenerator;
    }

    public async Task<Neo4j.User?> FindByAsync(string email, CancellationToken ct)
    {
        var result = (
            await _graphClient.Cypher.Match("(user:User)")
                .Where((Neo4j.User user) => user.Email == email)
                .Return(user => user.As<Neo4j.User>())
                .ResultsAsync
        ).ToList();
        return result.Any() ? result[0] : null;
    }

    public Task<Neo4j.User> LoginAsync(string email, string password, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task RegisterAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        string mobile,
        int? countryCode
    )
    {
        var encrypted = _passwordGenerator.Generate(password);
        await _graphClient.Cypher
            .Create(
                "(u:User {FirstName:$firstName, LastName:$lastName, Email:$email, Mobile:$mobile, CountryCode:$countryCode, Password:$password})")
            .WithParams(new Dictionary<string, object>
            {
                { "firstName", firstName },
                { "lastName", lastName },
                { "email", email },
                { "mobile", mobile },
                { "countryCode", countryCode },
                { "password", encrypted },
            }).ExecuteWithoutResultsAsync();
    }
}