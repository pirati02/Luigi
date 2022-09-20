using System.Text;
using System.Text.Json;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using User.Api.Service;

namespace User.Api.Feature.Register;

public class RegisterUserEventHandler : IAsyncMessageHandler
{
    private readonly IUserService _userService;

    public RegisterUserEventHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task Handle(MessageHandlingContext context, string matchingRoute)
    {
        context.AcknowledgeMessage();
        var body = Encoding.UTF8.GetString(context.Message.Body.ToArray());
        var registerUserCommand = JsonSerializer.Deserialize<RegisterUserCommand>(body);
        var endpoint = new RegisterUserCommandEndpoint(_userService);
        await endpoint.HandleAsync(registerUserCommand, CancellationToken.None);
        Console.WriteLine(context.Message);
    }
}