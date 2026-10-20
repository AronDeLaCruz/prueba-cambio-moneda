using System.Net;
using System.Text.Json;
using CambioMoneda.Domain.Exceptions;

namespace CambioMoneda.Api.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try { await next(ctx); }
        catch (Exception ex)
        {
            var (status, message) = ex switch
            {
                NotFoundException e   => (HttpStatusCode.NotFound, e.Message),
                BadRequestException e => (HttpStatusCode.BadRequest, e.Message),
                ConflictException e   => (HttpStatusCode.Conflict, e.Message),
                _                     => (HttpStatusCode.InternalServerError, "Ocurrió un error inesperado.")
            };

            if (status == HttpStatusCode.InternalServerError)
                logger.LogError(ex, "Error no controlado");

            ctx.Response.StatusCode = (int)status;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = (int)status,
                message,
                traceId = ctx.TraceIdentifier
            }));
        }
    }
}