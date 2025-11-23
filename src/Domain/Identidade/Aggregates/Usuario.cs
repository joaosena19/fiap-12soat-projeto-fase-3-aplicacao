using Domain.Identidade.Enums;
using Domain.Identidade.ValueObjects;
using Shared.Attributes;
using UUIDNext;

namespace Domain.Identidade.Aggregates
{
    [AggregateRoot]
    public class Usuario
    {
        public Guid Id { get; private set; }
        public DocumentoIdentificadorUsuario DocumentoIdentificadorUsuario { get; private set; } = null!;
        public SenhaHash SenhaHash { get; private set; } = null!;
        private readonly List<Role> _roles = new();
        public IReadOnlyList<Role> Roles => _roles.AsReadOnly();

        // Construtor sem parâmetro para EF Core
        private Usuario() { }

        private Usuario(Guid id, DocumentoIdentificadorUsuario documentoIdentificadorUsuario, SenhaHash senhaHash, List<Role> roles)
        {
            Id = id;
            DocumentoIdentificadorUsuario = documentoIdentificadorUsuario;
            SenhaHash = senhaHash;
            _roles.AddRange(roles);
        }

        public static Usuario Criar(string documento, string senhaHash, List<Role> roles)
        {
            return new Usuario(
                Uuid.NewSequential(), 
                new DocumentoIdentificadorUsuario(documento), 
                new SenhaHash(senhaHash),
                roles);
        }

        public static Usuario Criar(string documento, string senhaHash, Role role)
        {
            return Criar(documento, senhaHash, [role]);
        }
    }
}