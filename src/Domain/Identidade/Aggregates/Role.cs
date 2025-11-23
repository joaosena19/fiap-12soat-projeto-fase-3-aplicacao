using Domain.Identidade.Enums;
using Domain.Identidade.ValueObjects;
using Shared.Attributes;

namespace Domain.Identidade.Aggregates
{
    [AggregateMember]
    public class Role
    {
        public RoleEnum Id { get; private set; }
        public NomeRole Nome { get; private set; } = null!;

        // Construtor sem parâmetro para EF Core
        private Role() { }

        public Role(RoleEnum roleEnum)
        {
            Id = roleEnum;
            Nome = new NomeRole(roleEnum);
        }

        public static Role Administrador() => new(RoleEnum.Administrador);
        public static Role Cliente() => new(RoleEnum.Cliente);

        public static Role From(RoleEnum roleEnum) => new(roleEnum);
    }
}