using Application.Estoque.UseCases;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Domain.Estoque.Enums;

namespace Infrastructure.Handlers.Estoque
{
    public class ItemEstoqueHandler
    {
        public async Task CriarItemEstoqueAsync(Ator ator, string nome, int quantidade, TipoItemEstoqueEnum tipoItemEstoque, decimal preco, IItemEstoqueGateway gateway, ICriarItemEstoquePresenter presenter)
        {
            var useCase = new CriarItemEstoqueUseCase();
            await useCase.ExecutarAsync(ator, nome, quantidade, tipoItemEstoque, preco, gateway, presenter);
        }

        public async Task AtualizarItemEstoqueAsync(Ator ator, Guid id, string nome, int quantidade, TipoItemEstoqueEnum tipoItemEstoque, decimal preco, IItemEstoqueGateway gateway, IAtualizarItemEstoquePresenter presenter)
        {
            var useCase = new AtualizarItemEstoqueUseCase();
            await useCase.ExecutarAsync(ator, id, nome, quantidade, tipoItemEstoque, preco, gateway, presenter);
        }

        public async Task AtualizarQuantidadeAsync(Ator ator, Guid id, int quantidade, IItemEstoqueGateway gateway, IAtualizarQuantidadePresenter presenter)
        {
            var useCase = new AtualizarQuantidadeUseCase();
            await useCase.ExecutarAsync(ator, id, quantidade, gateway, presenter);
        }

        public async Task BuscarTodosItensEstoqueAsync(Ator ator, IItemEstoqueGateway gateway, IBuscarTodosItensEstoquePresenter presenter)
        {
            var useCase = new BuscarTodosItensEstoqueUseCase();
            await useCase.ExecutarAsync(ator, gateway, presenter);
        }

        public async Task BuscarItemEstoquePorIdAsync(Ator ator, Guid id, IItemEstoqueGateway gateway, IBuscarItemEstoquePorIdPresenter presenter)
        {
            var useCase = new BuscarItemEstoquePorIdUseCase();
            await useCase.ExecutarAsync(ator, id, gateway, presenter);
        }

        public async Task VerificarDisponibilidadeAsync(Ator ator, Guid id, int quantidadeRequisitada, IItemEstoqueGateway gateway, IVerificarDisponibilidadePresenter presenter)
        {
            var useCase = new VerificarDisponibilidadeUseCase();
            await useCase.ExecutarAsync(ator, id, quantidadeRequisitada, gateway, presenter);
        }
    }
}