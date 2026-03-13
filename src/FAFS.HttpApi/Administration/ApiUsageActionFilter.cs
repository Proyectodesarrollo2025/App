using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Volo.Abp.DependencyInjection;

namespace FAFS.Administration;

public class ApiUsageActionFilter : IAsyncActionFilter, ITransientDependency
{
    private readonly IApiUsageAppService _apiUsageAppService;

    public ApiUsageActionFilter(IApiUsageAppService apiUsageAppService)
    {
        _apiUsageAppService = apiUsageAppService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        
        var resultContext = await next(); // Ejecuta el endpoint
        
        stopwatch.Stop();

        // No grabamos métricas de la propia administración para no generar un bucle infinito de datos
        if (context.ActionDescriptor.DisplayName?.Contains("ApiUsage") == true)
        {
            return;
        }

        var endpoint = context.HttpContext.Request.Path.Value;
        var method = context.HttpContext.Request.Method;
        var statusCode = context.HttpContext.Response.StatusCode;
        var executionTime = stopwatch.Elapsed.TotalMilliseconds;
        var clientIp = context.HttpContext.Connection.RemoteIpAddress?.ToString();

        // Grabamos la métrica de forma asíncrona (fuego y olvido o Task.Run si no queremos bloquear el response)
        // En ABP lo ideal es usar BackgroundJob, pero por simplicidad para el TP lo hacemos directo
        await _apiUsageAppService.RecordMetricAsync(endpoint ?? "unknown", method, statusCode, executionTime, clientIp);
    }
}
