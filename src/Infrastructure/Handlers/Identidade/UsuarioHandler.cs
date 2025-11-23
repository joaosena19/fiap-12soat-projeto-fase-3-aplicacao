using Application.Identidade.Dtos;
using Application.Identidade.UseCases.Usuario;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;

namespace Infrastructure.Handlers.Identidade
{
    public class UsuarioHandler
    {
        public async Task CriarUsuarioAsync(CriarUsuarioDto dto, IUsuarioGateway gateway, ICriarUsuarioPresenter presenter)
        {
            var useCase = new CriarUsuarioUseCase();
            await useCase.ExecutarAsync(dto, gateway, presenter);
        }

        public async Task BuscarUsuarioPorDocumentoAsync(string documento, IUsuarioGateway gateway, IBuscarUsuarioPorDocumentoPresenter presenter)
        {
            var useCase = new BuscarUsuarioPorDocumentoUseCase();
            await useCase.ExecutarAsync(documento, gateway, presenter);
        }
    }
}