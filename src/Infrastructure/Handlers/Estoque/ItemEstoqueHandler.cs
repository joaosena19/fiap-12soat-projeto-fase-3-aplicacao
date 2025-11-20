using Application.Estoque.UseCases;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Domain.Estoque.Enums;

namespace Infrastructure.Handlers.Estoque
{
    public class ItemEstoqueHandler
    {
        public async Task CriarItemEstoqueAsync(string nome, int quantidade, TipoItemEstoqueEnum tipoItemEstoque, decimal preco, IItemEstoqueGateway gateway, ICriarItemEstoquePresenter presenter)
        {
            var useCase = new CriarItemEstoqueUseCase();
            await useCase.ExecutarAsync(nome, quantidade, tipoItemEstoque, preco, gateway, presenter);
        }

        public async Task AtualizarItemEstoqueAsync(Guid id, string nome, int quantidade, TipoItemEstoqueEnum tipoItemEstoque, decimal preco, IItemEstoqueGateway gateway, IAtualizarItemEstoquePresenter presenter)
        {
            var useCase = new AtualizarItemEstoqueUseCase();
            await useCase.ExecutarAsync(id, nome, quantidade, tipoItemEstoque, preco, gateway, presenter);
        }

        public async Task AtualizarQuantidadeAsync(Guid id, int quantidade, IItemEstoqueGateway gateway, IAtualizarQuantidadePresenter presenter)
        {
            var useCase = new AtualizarQuantidadeUseCase();
            await useCase.ExecutarAsync(id, quantidade, gateway, presenter);
        }

        public async Task BuscarTodosItensEstoqueAsync(IItemEstoqueGateway gateway, IBuscarTodosItensEstoquePresenter presenter)
        {
            var useCase = new BuscarTodosItensEstoqueUseCase();
            await useCase.ExecutarAsync(gateway, presenter);
        }

        public async Task BuscarItemEstoquePorIdAsync(Guid id, IItemEstoqueGateway gateway, IBuscarItemEstoquePorIdPresenter presenter)
        {
            var useCase = new BuscarItemEstoquePorIdUseCase();
            await useCase.ExecutarAsync(id, gateway, presenter);
        }

        public async Task VerificarDisponibilidadeAsync(Guid id, int quantidadeRequisitada, IItemEstoqueGateway gateway, IVerificarDisponibilidadePresenter presenter)
        {
            var useCase = new VerificarDisponibilidadeUseCase();
            await useCase.ExecutarAsync(id, quantidadeRequisitada, gateway, presenter);
        }
    }
}