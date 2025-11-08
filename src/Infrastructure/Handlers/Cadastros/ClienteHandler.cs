using Application.Cadastros.UseCases;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;

namespace Infrastructure.Handlers.Cadastros
{
    public class ClienteHandler
    {
        public async Task CriarClienteAsync(string nome, string documento, IClienteGateway gateway, ICriarClientePresenter presenter)
        {
            var useCase = new CriarClienteUseCase();
            await useCase.ExecutarAsync(nome, documento, gateway, presenter);
        }

        public async Task AtualizarClienteAsync(Guid id, string nome, IClienteGateway gateway, IAtualizarClientePresenter presenter)
        {
            var useCase = new AtualizarClienteUseCase();
            await useCase.ExecutarAsync(id, nome, gateway, presenter);
        }

        public async Task BuscarClientesAsync(IClienteGateway gateway, IBuscarClientesPresenter presenter)
        {
            var useCase = new BuscarClientesUseCase();
            await useCase.ExecutarAsync(gateway, presenter);
        }

        public async Task BuscarClientePorIdAsync(Guid id, IClienteGateway gateway, IBuscarClientePorIdPresenter presenter)
        {
            var useCase = new BuscarClientePorIdUseCase();
            await useCase.ExecutarAsync(id, gateway, presenter);
        }

        public async Task BuscarClientePorDocumentoAsync(string documento, IClienteGateway gateway, IBuscarClientePorDocumentoPresenter presenter)
        {
            var useCase = new BuscarClientePorDocumentoUseCase();
            await useCase.ExecutarAsync(documento, gateway, presenter);
        }
    }
}