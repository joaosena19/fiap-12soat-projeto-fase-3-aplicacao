using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Domain.Cadastros.Aggregates;
using Shared.Exceptions;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class CriarClienteUseCase
    {
        public async Task ExecutarAsync(string nome, string documento, IClienteGateway gateway, ICriarClientePresenter presenter)
        {
            try
            {
                var clienteExistente = await gateway.ObterPorDocumentoAsync(documento);
                if (clienteExistente != null)
                {
                    presenter.ApresentarErro("Já existe um cliente cadastrado com este documento.", ErrorType.Conflict);
                    return;
                }

                var novoCliente = Cliente.Criar(nome, documento);
                var clienteSalvo = await gateway.SalvarAsync(novoCliente);

                presenter.ApresentarSucesso(clienteSalvo);
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