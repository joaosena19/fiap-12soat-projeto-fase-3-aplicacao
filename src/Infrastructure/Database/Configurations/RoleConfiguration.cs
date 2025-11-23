using Domain.Identidade.Enums;
using Domain.Identidade.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("roles");
            builder.HasKey(r => r.RoleEnum);

            builder.Property(r => r.RoleEnum)
                   .HasColumnName("id")
                   .HasConversion(
                       v => (int)v,
                       v => (RoleEnum)v
                   );

            builder.Property(r => r.Nome)
                   .HasColumnName("nome")
                   .IsRequired()
                   .HasMaxLength(50);

            // Seed data para as roles
            builder.HasData(
                new { RoleEnum = RoleEnum.Administrador, Nome = RoleEnum.Administrador.ToString() },
                new { RoleEnum = RoleEnum.Cliente, Nome = RoleEnum.Cliente.ToString() }
            );
        }
    }
}