using CambioMoneda.Application.Dtos;
using CambioMoneda.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CambioMoneda.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IJwtTokenGenerator jwt, IConfiguration config) : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login(LoginRequest r)
    {
        if (r.Usuario != config["Auth:Usuario"] || r.Password != config["Auth:Password"])
            return Unauthorized(new { status = 401, message = "Credenciales inválidas." });

        return jwt.Generate(r.Usuario);
    }
}