using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Contracts.Services;
using Application.Identidade.Dtos;
using Domain.Identidade.Aggregates;
using Domain.Identidade.ValueObjects;
using Shared.Enums;
using Shared.Exceptions;
using UsuarioAggregate = Domain.Identidade.Aggregates.Usuario;

namespace Application.Identidade.UseCases.Usuario
{
    public class CriarUsuarioUseCase
    {
        public async Task ExecutarAsync(CriarUsuarioDto dto, IUsuarioGateway gateway, ICriarUsuarioPresenter presenter, IPasswordHasher passwordHasher)
        {
            try
            {
                var usuarioExistente = await gateway.ObterPorDocumentoAsync(dto.DocumentoIdentificador);
                if (usuarioExistente != null)
                {
                    presenter.ApresentarErro("Já existe um usuário cadastrado com este documento.", ErrorType.Conflict);
                    return;
                }

                // Busca as roles existentes no banco ao invés de criar novas instâncias
                var roles = await gateway.ObterRolesAsync(dto.Roles);
                
                if (roles.Count != dto.Roles.Count)
                {
                    presenter.ApresentarErro("Uma ou mais roles informadas são inválidas.", ErrorType.InvalidInput);
                    return;
                }

                var senhaHasheada = passwordHasher.Hash(dto.SenhaNaoHasheada);
                var senhaHash = new SenhaHash(senhaHasheada);

                var novoUsuario = UsuarioAggregate.Criar(dto.DocumentoIdentificador, senhaHash.Valor, roles);
                var usuarioSalvo = await gateway.SalvarAsync(novoUsuario);

                presenter.ApresentarSucesso(usuarioSalvo);
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