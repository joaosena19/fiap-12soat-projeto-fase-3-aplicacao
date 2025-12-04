using Application.Identidade.Services;
using Infrastructure.Authentication.AtorFactories;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public abstract class BaseController : ControllerBase
{
    protected Ator Ator { get; private set; } = null!;

    protected BaseController()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return;

        var token = authHeader.Replace("Bearer ", "");
        Ator = AtorJwtFactory.CriarPorTokenJwt(token);
    }
}