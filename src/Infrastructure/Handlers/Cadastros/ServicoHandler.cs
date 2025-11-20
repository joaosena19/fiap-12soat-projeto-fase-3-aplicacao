using Application.Cadastros.UseCases;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;

namespace Infrastructure.Handlers.Cadastros
{
    public class ServicoHandler
    {
        public async Task CriarServicoAsync(string nome, decimal preco, IServicoGateway gateway, ICriarServicoPresenter presenter)
        {
            var useCase = new CriarServicoUseCase();
            await useCase.ExecutarAsync(nome, preco, gateway, presenter);
        }

        public async Task AtualizarServicoAsync(Guid id, string nome, decimal preco, IServicoGateway gateway, IAtualizarServicoPresenter presenter)
        {
            var useCase = new AtualizarServicoUseCase();
            await useCase.ExecutarAsync(id, nome, preco, gateway, presenter);
        }

        public async Task BuscarServicosAsync(IServicoGateway gateway, IBuscarServicosPresenter presenter)
        {
            var useCase = new BuscarServicosUseCase();
            await useCase.ExecutarAsync(gateway, presenter);
        }

        public async Task BuscarServicoPorIdAsync(Guid id, IServicoGateway gateway, IBuscarServicoPorIdPresenter presenter)
        {
            var useCase = new BuscarServicoPorIdUseCase();
            await useCase.ExecutarAsync(id, gateway, presenter);
        }
    }
}