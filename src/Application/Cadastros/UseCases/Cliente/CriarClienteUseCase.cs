using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Domain.Cadastros.Aggregates;
using Shared.Exceptions;
using Shared.Enums;

namespace Application.Cadastros.UseCases
{
    public class CriarClienteUseCase
    {
        public async Task ExecutarAsync(Ator ator, string nome, string documento, IClienteGateway clienteGateway, IUsuarioGateway usuarioGateway, ICriarClientePresenter presenter)
        {
            try
            {
                if (!await ator.PodeCriarClienteAsync(documento, usuarioGateway))
                {
                    presenter.ApresentarErro("Acesso negado. Administradores podem cadastrar qualquer cliente, usuários podem criar cliente apenas com o mesmo documento.", ErrorType.NotAllowed);
                    return;
                }

                var clienteExistente = await clienteGateway.ObterPorDocumentoAsync(documento);
                if (clienteExistente != null)
                {
                    presenter.ApresentarErro("Já existe um cliente cadastrado com este documento.", ErrorType.Conflict);
                    return;
                }

                var novoCliente = Cliente.Criar(nome, documento);
                var clienteSalvo = await clienteGateway.SalvarAsync(novoCliente);

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