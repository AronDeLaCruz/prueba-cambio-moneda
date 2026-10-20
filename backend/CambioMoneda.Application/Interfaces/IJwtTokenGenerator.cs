using CambioMoneda.Application.Dtos;
using CambioMoneda.Domain.Entities;

namespace CambioMoneda.Application.Interfaces;

public interface IJwtTokenGenerator
{
    LoginResponse Generate(string usuario);
}