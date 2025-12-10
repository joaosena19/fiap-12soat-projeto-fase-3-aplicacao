using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Shared.Exceptions;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class AtualizarServicoUseCase
    {
        public async Task ExecutarAsync(Ator ator, Guid id, string nome, decimal preco, IServicoGateway gateway, IAtualizarServicoPresenter presenter)
        {
            try
            {
                if (!ator.PodeGerenciarServicos())
                {
                    presenter.ApresentarErro("Acesso negado. Apenas administradores podem gerenciar serviços.", ErrorType.NotAllowed);
                    return;
                }

                var servico = await gateway.ObterPorIdAsync(id);
                if (servico == null)
                {
                    presenter.ApresentarErro("Serviço não encontrado.", ErrorType.ResourceNotFound);
                    return;
                }

                servico.Atualizar(nome, preco);
                var servicoAtualizado = await gateway.AtualizarAsync(servico);

                presenter.ApresentarSucesso(servicoAtualizado);
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