using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Exceptions;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class AtualizarClienteUseCase
    {
        public async Task ExecutarAsync(Ator ator, Guid id, string nome, IClienteGateway gateway, IAtualizarClientePresenter presenter)
        {
            try
            {
                var cliente = await gateway.ObterPorIdAsync(id);
                if (cliente == null)
                {
                    presenter.ApresentarErro("Cliente não encontrado.", ErrorType.ResourceNotFound);
                    return;
                }

                if (!ator.PodeEditarCliente(cliente.Id))
                {
                    presenter.ApresentarErro("Acesso negado. Somente administradores ou o próprio cliente podem editar os dados.", ErrorType.NotAllowed);
                    return;
                }

                cliente.Atualizar(nome);
                var clienteAtualizado = await gateway.AtualizarAsync(cliente);

                presenter.ApresentarSucesso(clienteAtualizado);
            }
            catch (DomainException ex)
            {
                presenter.ApresentarErro(ex.Message, ex.ErrorType);
            }
            catch (Exception)
            {
                presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            }
        }
    }
}