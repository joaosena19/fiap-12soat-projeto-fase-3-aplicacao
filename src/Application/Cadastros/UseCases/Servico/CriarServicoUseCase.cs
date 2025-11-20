using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Domain.Cadastros.Aggregates;
using Shared.Exceptions;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class CriarServicoUseCase
    {
        public async Task ExecutarAsync(string nome, decimal preco, IServicoGateway gateway, ICriarServicoPresenter presenter)
        {
            try
            {
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