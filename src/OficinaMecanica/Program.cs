using API.Configurations;
using API.Configurations.Swagger;
using API.Middleware;
using Serilog;
using NewRelic.LogEnrichers.Serilog;

// Configurar Serilog com New Relic e propriedades contextuais
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build())
    .Enrich.FromLogContext()
    .Enrich.WithNewRelicLogsInContext()
    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j} {NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Usar Serilog como provedor de logs
builder.Host.UseSerilog();

builder.Services.AddApiControllers();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddHealthChecks();


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwaggerDocumentation();
app.UseHttpsRedirection();
app.UseSecurityHeadersConfiguration();
app.UseAuthentication();
app.UseAuthorization();
app.UseHealthCheckEndpoints();
app.MapControllers();


// Popula dados mock
DevelopmentDataSeeder.Seed(app);

await app.RunAsync();

//Necessário para testes de integração
public partial class Program { }

