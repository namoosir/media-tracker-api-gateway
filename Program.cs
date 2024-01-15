// using MediaTrackerApiGateway.Controllers;
// using MediaTrackerApiGateway.Data;
using MediaTrackerApiGateway.Middleware.DelegatingHandlers;
using MediaTrackerApiGateway.Middleware;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using StackExchange.Redis;
using MediaTrackerApiGateway.Services.SessionTokenService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(logging =>
{
    logging.AddConsole(); // Use the console logger
});

// Add services to the container.
builder.Services.AddSingleton<IConnectionMultiplexer>(
    opt =>
        ConnectionMultiplexer.Connect(
            builder.Configuration.GetConnectionString("RedisConnectionString")
        )
);

// builder.Services.AddSingleton<IUserInformationRepository, UserInformationRepository>();
// builder.Services.AddSingleton<UserInformationController>();
builder.Services.AddSingleton<ISessionTokenService, SessionTokenService>();
builder.Services.AddScoped<CustomAuthenticationHandler>();

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddOcelot(builder.Configuration).AddDelegatingHandler<ApendUserIdFromToken>();

// builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var configuration = new OcelotPipelineConfiguration
{
    AuthenticationMiddleware = async (ctx, next) =>
    {
        var customAuthenticationHandler =
            ctx.RequestServices.GetRequiredService<CustomAuthenticationHandler>();
        await customAuthenticationHandler.HandleAsync(ctx, next);
    },
};

await app.UseOcelot(configuration);

app.Run();
