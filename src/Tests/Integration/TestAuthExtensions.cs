using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Integration
{
    public static class TestAuthExtensions
    {
        public static HttpClient CreateAuthenticatedClient<TEntryPoint>(this WebApplicationFactory<TEntryPoint> factory)
            where TEntryPoint : class
        {
            // Cria uma nova factory com a configuração de autenticação de teste
            var authenticatedFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Configura o esquema de autenticação de teste
                    services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            TestAuthHandler.AuthenticationScheme, options => { });
                });
            });

            // Retorna um cliente dessa factory configurada
            return authenticatedFactory.CreateClient();
        }
    }
}
