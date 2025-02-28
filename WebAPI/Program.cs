using Persistence;
using Serilog;
using Serilog.Events;
using Application;
using WebAPI.Middlewares;
using Identity;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// logs data collected during the app startup (using serilog)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Seq("http://localhost:5341")
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting up");

    // Add services to the container.
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // add global exception handling middleware
    builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

    // add all layers services using dependency injection
    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration)
        .AddIdentity(builder.Configuration);

    // Configure CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ReactClient",
            corsPolicyBuilder => corsPolicyBuilder
                .WithOrigins("http://localhost:5173")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
    });

    // Configure Serilog with Seq from appsettings
    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Seq(
                serverUrl: context.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341")
            .Destructure.ByTransforming<Exception>(ex => new { ex.Message, ex.StackTrace })); 

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Move CORS middleware before other middleware
    app.UseCors("ReactClient");

    // specify which middleware to use
    app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

    // log http request
    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            // log host name, http/s protocol, request maker
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"]);
        };
    });

    // Apply CORS policy
    app.UseCors("ReactClient");

    // seed roles and admin user
    //using (var scope = app.Services.CreateScope())
    //{
    //    var services = scope.ServiceProvider;
    //    await Identity.DependencyInjection.SeedDataAsync(services);
    //}

    app.UseHttpsRedirection();

    var jwtSettings = builder.Configuration.GetSection("JwtSettings");

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    await app.RunAsync();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
