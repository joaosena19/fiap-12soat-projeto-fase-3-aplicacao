using Application.Cadastros.UseCases;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Domain.Cadastros.Enums;

namespace Infrastructure.Handlers.Cadastros
{
    public class VeiculoHandler
    {
        public async Task CriarVeiculoAsync(Guid clienteId, string placa, string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipoVeiculo,
            IVeiculoGateway veiculoGateway, IClienteGateway clienteGateway, ICriarVeiculoPresenter presenter)
        {
            var useCase = new CriarVeiculoUseCase();
            await useCase.ExecutarAsync(clienteId, placa, modelo, marca, cor, ano, tipoVeiculo, veiculoGateway, clienteGateway, presenter);
        }

        public async Task AtualizarVeiculoAsync(Ator ator, Guid id, string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipoVeiculo,
            IVeiculoGateway gateway, IAtualizarVeiculoPresenter presenter)
        {
            var useCase = new AtualizarVeiculoUseCase();
            await useCase.ExecutarAsync(ator, id, modelo, marca, cor, ano, tipoVeiculo, gateway, presenter);
        }

        public async Task BuscarVeiculosAsync(IVeiculoGateway gateway, IBuscarVeiculosPresenter presenter)
        {
            var useCase = new BuscarVeiculosUseCase();
            await useCase.ExecutarAsync(gateway, presenter);
        }

        public async Task BuscarVeiculoPorIdAsync(Ator ator, Guid id, IVeiculoGateway gateway, IBuscarVeiculoPorIdPresenter presenter)
        {
            var useCase = new BuscarVeiculoPorIdUseCase();
            await useCase.ExecutarAsync(ator, id, gateway, presenter);
        }

        public async Task BuscarVeiculoPorPlacaAsync(Ator ator, string placa, IVeiculoGateway gateway, IBuscarVeiculoPorPlacaPresenter presenter)
        {
            var useCase = new BuscarVeiculoPorPlacaUseCase();
            await useCase.ExecutarAsync(ator, placa, gateway, presenter);
        }

        public async Task BuscarVeiculosPorClienteAsync(Guid clienteId, IVeiculoGateway veiculoGateway, IClienteGateway clienteGateway, IBuscarVeiculosPorClientePresenter presenter)
        {
            var useCase = new BuscarVeiculosPorClienteUseCase();
            await useCase.ExecutarAsync(clienteId, veiculoGateway, clienteGateway, presenter);
        }
    }
}