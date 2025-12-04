using Domain.Cadastros.Aggregates;

namespace Application.Identidade.Services.Extensions;

public static class AtorClienteExtensions
{
    /// <summary>
    /// Administrador ou próprio cliente
    /// </summary>
    public static bool PodeAcessarCliente(this Ator ator, Cliente cliente)
    {
        return ator.PodeGerenciarSistema() || cliente.Id == ator.ClienteId;
    }

    /// <summary>
    /// Administrador ou próprio cliente
    /// </summary>
    public static bool PodeAcessarCliente(this Ator ator, Guid clienteId)
    {
        return ator.PodeGerenciarSistema() || clienteId == ator.ClienteId;
    }

    /// <summary>
    /// Somente administrador
    /// </summary>
    public static bool PodeCriarCliente(this Ator ator)
    {
        return ator.PodeGerenciarSistema();
    }

    /// <summary>
    /// Somente administrador
    /// </summary>
    public static bool PodeListarClientes(this Ator ator)
    {
        return ator.PodeGerenciarSistema();
    }

    /// <summary>
    /// Administrador ou próprio cliente
    /// </summary>
    public static bool PodeEditarCliente(this Ator ator, Guid clienteId)
    {
        return ator.PodeGerenciarSistema() || ator.ClienteId == clienteId;
    }
}