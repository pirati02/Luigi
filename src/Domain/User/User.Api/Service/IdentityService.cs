using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Neo4jClient;

namespace User.Api.Service;

public interface IIdentityService
{
    Task AuthenticateAsync(string email, string password, HttpContext httpContext);
}

public class IdentityService : IIdentityService
{
    private readonly IPasswordGenerator _passwordGenerator;
    private readonly IGraphClient _graphClient;

    public IdentityService(
        IPasswordGenerator passwordGenerator,
        IGraphClient graphClient
    )
    {
        _passwordGenerator = passwordGenerator;
        _graphClient = graphClient;
    }

    public async Task AuthenticateAsync(string email, string password, HttpContext httpContext)
    {
        var encrypted = _passwordGenerator.Generate(password);
        var user = await GetUser(email, encrypted);
        if (user is null)
        {
            return;
        }

        var jsonUser = JsonSerializer.Serialize(user);
        var claims = new List<Claim>
        {
            new("user", jsonUser),
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "local"));
        await httpContext.SignInAsync(principal);
    }

    private async Task<Neo4j.User?> GetUser(string email, string encrypted)
    {
        var result = (
            await _graphClient.Cypher.Match("(user:User)")
                .Where((Neo4j.User user) => user.Email == email && user.Password == encrypted)
                .Return(user => user.As<Neo4j.User>())
                .ResultsAsync
        ).ToList();
        return result.Any() ? result[0] : null;
    }
}