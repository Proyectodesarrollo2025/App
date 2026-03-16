using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace FAFS.Administration;

public interface IApiUsageAppService : IApplicationService
{
    Task<ApiUsageStatisticsDto> GetStatisticsAsync();
    Task RecordMetricAsync(string endpoint, string method, int statusCode, double executionTime, string? clientIp = null);
}
