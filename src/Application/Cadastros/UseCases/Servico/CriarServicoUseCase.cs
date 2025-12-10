using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Domain.Cadastros.Aggregates;
using Shared.Exceptions;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class CriarServicoUseCase
    {
        public async Task ExecutarAsync(Ator ator, string nome, decimal preco, IServicoGateway gateway, ICriarServicoPresenter presenter)
        {
            try
            {
                if (!ator.PodeGerenciarServicos())
                {
                    presenter.ApresentarErro("Acesso negado. Apenas administradores podem gerenciar serviços.", ErrorType.NotAllowed);
                    return;
                }

                var servicoExistente = await gateway.ObterPorNomeAsync(nome);
                if (servicoExistente != null)
                {
                    presenter.ApresentarErro("Já existe um serviço cadastrado com este nome.", ErrorType.Conflict);
                    return;
                }

                var novoServico = Servico.Criar(nome, preco);
                var servicoSalvo = await gateway.SalvarAsync(novoServico);

                presenter.ApresentarSucesso(servicoSalvo);
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