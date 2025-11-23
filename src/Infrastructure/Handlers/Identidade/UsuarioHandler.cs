using Application.Identidade.Dtos;
using Application.Identidade.UseCases.Usuario;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Contracts.Services;

namespace Infrastructure.Handlers.Identidade
{
    public class UsuarioHandler
    {
        public async Task CriarUsuarioAsync(CriarUsuarioDto dto, IUsuarioGateway gateway, ICriarUsuarioPresenter presenter, IPasswordHasher passwordHasher)
        {
            var useCase = new CriarUsuarioUseCase();
            await useCase.ExecutarAsync(dto, gateway, presenter, passwordHasher);
        }

        public async Task BuscarUsuarioPorDocumentoAsync(string documento, IUsuarioGateway gateway, IBuscarUsuarioPorDocumentoPresenter presenter)
        {
            var useCase = new BuscarUsuarioPorDocumentoUseCase();
            await useCase.ExecutarAsync(documento, gateway, presenter);
        }
    }
}