using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace FAFS.Administration;

public class ApiUsageAppService : ApplicationService, IApiUsageAppService
{
    private readonly IRepository<ApiUsageMetric, Guid> _repository;

    public ApiUsageAppService(IRepository<ApiUsageMetric, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<ApiUsageStatisticsDto> GetStatisticsAsync()
    {
        var metrics = await _repository.GetListAsync();

        if (!metrics.Any())
        {
            return new ApiUsageStatisticsDto
            {
                MostUsedEndpoints = new List<EndpointExecutionDto>()
            };
        }

        var stats = new ApiUsageStatisticsDto
        {
            TotalCalls = metrics.Count,
            AverageExecutionTime = metrics.Average(x => x.ExecutionTime),
            SuccessCount = metrics.Count(x => x.StatusCode >= 200 && x.StatusCode < 300),
            ErrorCount = metrics.Count(x => x.StatusCode >= 400),
            MostUsedEndpoints = metrics
                .GroupBy(x => x.Endpoint)
                .Select(g => new EndpointExecutionDto
                {
                    Endpoint = g.Key,
                    CallCount = g.Count(),
                    AverageExecutionTime = g.Average(x => x.ExecutionTime)
                })
                .OrderByDescending(x => x.CallCount)
                .Take(10)
                .ToList()
        };

        return stats;
    }

    public async Task RecordMetricAsync(string endpoint, string method, int statusCode, double executionTime, string? clientIp = null)
    {
        var metric = new ApiUsageMetric(
            Guid.NewGuid(), // Usamos Guid.NewGuid() para simplificar tests unitarios manuales
            endpoint,
            method,
            statusCode,
            executionTime,
            clientIp
        );

        await _repository.InsertAsync(metric);
    }
}
