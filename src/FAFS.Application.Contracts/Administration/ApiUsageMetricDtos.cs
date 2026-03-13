using System;
using System.Collections.Generic;

namespace FAFS.Administration;

public class ApiUsageMetricDto
{
    public Guid Id { get; set; }
    public string Endpoint { get; set; } = default!;
    public string Method { get; set; } = default!;
    public int StatusCode { get; set; }
    public double ExecutionTime { get; set; }
    public string? ClientIp { get; set; }
    public DateTime CreationTime { get; set; }
}

public class ApiUsageStatisticsDto
{
    public long TotalCalls { get; set; }
    public double AverageExecutionTime { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public List<EndpointExecutionDto> MostUsedEndpoints { get; set; } = new();
}

public class EndpointExecutionDto
{
    public string Endpoint { get; set; } = default!;
    public int CallCount { get; set; }
    public double AverageExecutionTime { get; set; }
}
