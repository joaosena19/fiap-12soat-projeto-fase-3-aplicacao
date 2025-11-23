using Domain.Identidade.Enums;
using Shared.Attributes;

namespace Domain.Identidade.ValueObjects
{
    [ValueObject]
    public record Role
    {
        public RoleEnum RoleEnum { get; private init; }
        public string Nome { get; private init; } = string.Empty;

        // Construtor sem parâmetro para EF Core
        private Role() { }

        private Role(RoleEnum roleEnum)
        {
            RoleEnum = roleEnum;
            Nome = roleEnum.ToString();
        }

        public static Role Administrador() => new(RoleEnum.Administrador);
        public static Role Cliente() => new(RoleEnum.Cliente);

        public static Role From(RoleEnum roleEnum) => roleEnum switch
        {
            RoleEnum.Administrador => Administrador(),
            RoleEnum.Cliente => Cliente(),
            _ => throw new ArgumentException($"Tipo de role inválido: {roleEnum}")
        };
    }
}