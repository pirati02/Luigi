using FastEndpoints;
using Marten;
using Marten.Events;
using Marten.Events.Projections;
using Microsoft.EntityFrameworkCore;
using Nest;
using Reservation.Api.Infrastructure;
using Reservation.Api.Infrastructure.ElasticSearch;
using Reservation.Api.Logging;
using Reservation.Api.Service;
using Reservation.Infrastructure;
using Reservation.Infrastructure.Projection;
using Reservation.Settings.Infrastructure;
using Serilog;
using Weasel.Core;

KibanaLoggingExtension.ConfigureLogging();

var builder = WebApplication.CreateBuilder(args);
builder.WebHost
    .ConfigureAppConfiguration(configuration =>
    {
        configuration.AddJsonFile("Infrastructure/ConnectionStrings/kibana.json", optional: false, reloadOnChange: true);
        configuration.AddJsonFile("Infrastructure/ConnectionStrings/rabbitmq.json", optional: false, reloadOnChange: true);
        configuration.AddJsonFile("Infrastructure/ConnectionStrings/postgres.json", optional: false, reloadOnChange: true);
    }).UseSerilog();

builder.Services.AddScoped<AggregateRepository>();
builder.Services.AddDbContextPool<ReservationConfigurationDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("ReservationDb"));
});
builder.Services.AddMarten(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("ReservationEventDb");
    opt.Connection(connectionString);
    opt.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
    opt.Events.StreamIdentity = StreamIdentity.AsGuid;
    opt.DatabaseSchemaName = "events";
    opt.Projections.Add<ReservationProjection>(ProjectionLifecycle.Async);
}); 
builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDocument();
builder.Services.AddSingleton<IElasticClient>(_ => ElasticClientFactory.BuildElasticClient(builder.Configuration));
builder.Services.AddScoped<IActiveReservationFinder, ActiveReservationFinder>();
var app = builder.Build();

app.UseFastEndpoints();
app.UseApimundo();
app.UseSwaggerUi3();
app.Run();