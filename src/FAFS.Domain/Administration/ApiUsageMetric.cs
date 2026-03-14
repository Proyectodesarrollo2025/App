using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace FAFS.Administration;

public class ApiUsageMetric : CreationAuditedEntity<Guid>
{
    public string Endpoint { get; set; } = default!;
    public string Method { get; set; } = default!;
    public int StatusCode { get; set; }
    public double ExecutionTime { get; set; } // En milisegundos
    public string? ClientIp { get; set; }

    protected ApiUsageMetric()
    {
    }

    public ApiUsageMetric(
        Guid id,
        string endpoint,
        string method,
        int statusCode,
        double executionTime,
        string? clientIp = null)
        : base(id)
    {
        Endpoint = endpoint;
        Method = method;
        StatusCode = statusCode;
        ExecutionTime = executionTime;
        ClientIp = clientIp;
    }
}
