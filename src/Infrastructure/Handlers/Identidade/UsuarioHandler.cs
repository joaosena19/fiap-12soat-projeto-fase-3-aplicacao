using Application.Identidade.Dtos;
using Application.Identidade.Services;
using Application.Identidade.UseCases.Usuario;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Contracts.Services;

namespace Infrastructure.Handlers.Identidade
{
    public class UsuarioHandler
    {
        public async Task CriarUsuarioAsync(Ator ator, CriarUsuarioDto dto, IUsuarioGateway gateway, ICriarUsuarioPresenter presenter, IPasswordHasher passwordHasher)
        {
            var useCase = new CriarUsuarioUseCase();
            await useCase.ExecutarAsync(ator, dto, gateway, presenter, passwordHasher);
        }

        public async Task BuscarUsuarioPorDocumentoAsync(Ator ator, string documento, IUsuarioGateway gateway, IBuscarUsuarioPorDocumentoPresenter presenter)
        {
            var useCase = new BuscarUsuarioPorDocumentoUseCase();
            await useCase.ExecutarAsync(ator, documento, gateway, presenter);
        }
    }
}