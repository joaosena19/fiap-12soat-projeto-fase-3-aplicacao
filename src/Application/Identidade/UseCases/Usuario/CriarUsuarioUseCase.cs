using Application.Identidade.Dtos;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Domain.Identidade.Aggregates;
using Domain.Identidade.ValueObjects;
using Domain.Identidade.Enums;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.Identidade.UseCases.Usuario
{
    public class CriarUsuarioUseCase
    {
        public async Task ExecutarAsync(CriarUsuarioDto dto, IUsuarioGateway gateway, ICriarUsuarioPresenter presenter)
        {
            try
            {
                var usuarioExistente = await gateway.ObterPorDocumentoAsync(dto.DocumentoIdentificador);
                if (usuarioExistente != null)
                {
                    presenter.ApresentarErro("Já existe um usuário cadastrado com este documento.", ErrorType.Conflict);
                    return;
                }

                // Converte as strings de roles para enum
                var roles = new List<Role>();
                foreach (var roleString in dto.Roles)
                {
                    if (Enum.TryParse<RoleEnum>(roleString, out var roleEnum))
                    {
                        roles.Add(Role.From(roleEnum));
                    }
                    else
                    {
                        presenter.ApresentarErro($"Role inválido: {roleString}", ErrorType.InvalidInput);
                        return;
                    }
                }

                // Cria hash da senha
                var senhaHash = new SenhaHash(dto.SenhaNaoCriptografada);

                var novoUsuario = Domain.Identidade.Aggregates.Usuario.Criar(dto.DocumentoIdentificador, senhaHash.Valor, roles);
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