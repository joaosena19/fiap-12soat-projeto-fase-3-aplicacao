using API.Configurations;
using API.Middleware;

var builder = WebApplication.CreateBuilder(args);

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


// Popula dados mock se estiver em ambiente de desenvolvimento
DevelopmentDataSeeder.SeedIfDevelopment(app);

await app.RunAsync();

//Necessário para testes de integração
public partial class Program { }

