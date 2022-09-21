using Neo4jClient;

namespace User.Api.Service;

public interface IUserService
{
    Task<Neo4j.User?> FindByAsync(string email);

    Task RegisterAsync(Guid id,
        string firstName,
        string lastName,
        string email,
        string password,
        string mobile,
        int? countryCode);
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

    public async Task<Neo4j.User?> FindByAsync(string email)
    {
        var result = (
            await _graphClient.Cypher.Match("(user:User)")
                .Where((Neo4j.User user) => user.Email == email)
                .Return(user => user.As<Neo4j.User>())
                .ResultsAsync
        ).ToList();
        return result.Any() ? result[0] : null;
    }

    public async Task RegisterAsync(Guid id,
        string firstName,
        string lastName,
        string email,
        string password,
        string mobile,
        int? countryCode)
    {
        var encrypted = string.IsNullOrWhiteSpace(password)
            ? string.Empty
            : _passwordGenerator.Generate(password);
        var notActive = string.IsNullOrWhiteSpace(password);

        await _graphClient.Cypher
            .Create(
                "(u:User {Id:$id, FirstName:$firstName, LastName:$lastName, Email:$email, Mobile:$mobile, CountryCode:$countryCode, Password:$password, NotActive:$notActive})")
            .WithParams(new Dictionary<string, object>
            {
                { "id", id },
                { "firstName", firstName },
                { "lastName", lastName },
                { "email", email },
                { "mobile", mobile },
                { "countryCode", countryCode },
                { "password", encrypted },
                { "notActive", notActive }
            }).ExecuteWithoutResultsAsync();
    }
}