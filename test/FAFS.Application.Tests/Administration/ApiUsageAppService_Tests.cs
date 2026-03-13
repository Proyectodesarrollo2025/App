using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace FAFS.Administration;

public class ApiUsageAppService_Tests
{
    private readonly Mock<IRepository<ApiUsageMetric, Guid>> _mockRepository;
    private readonly ApiUsageAppService _appService;

    public ApiUsageAppService_Tests()
    {
        _mockRepository = new Mock<IRepository<ApiUsageMetric, Guid>>();
        _appService = new ApiUsageAppService(_mockRepository.Object);
    }

    [Fact]
    public async Task RecordMetricAsync_Should_Insert_Metric()
    {
        // Act
        await _appService.RecordMetricAsync("/api/test", "GET", 200, 150.5, "127.0.0.1");

        // Assert
        _mockRepository.Verify(x => x.InsertAsync(It.Is<ApiUsageMetric>(m =>
            m!.Endpoint == "/api/test" &&
            m!.Method == "GET" &&
            m!.StatusCode == 200 &&
            m!.ExecutionTime == 150.5 &&
            m!.ClientIp == "127.0.0.1"
        ), It.IsAny<bool>(), default), Times.Once);
    }

    [Fact]
    public async Task GetStatisticsAsync_Should_Return_Correct_Stats()
    {
        // Arrange
        var metrics = new List<ApiUsageMetric>
        {
            new ApiUsageMetric(Guid.NewGuid(), "/api/a", "GET", 200, 100),
            new ApiUsageMetric(Guid.NewGuid(), "/api/a", "GET", 200, 200),
            new ApiUsageMetric(Guid.NewGuid(), "/api/b", "POST", 500, 300)
        };

        _mockRepository.Setup(x => x.GetListAsync(false, default)).ReturnsAsync(metrics);

        // Act
        var stats = await _appService.GetStatisticsAsync();

        // Assert
        stats.ShouldNotBeNull();
        stats!.TotalCalls.ShouldBe(3);
        stats!.AverageExecutionTime.ShouldBe(200); // (100+200+300)/3
        stats!.SuccessCount.ShouldBe(2);
        stats!.ErrorCount.ShouldBe(1);
        stats!.MostUsedEndpoints.Count.ShouldBe(2);
        
        var apiA = stats!.MostUsedEndpoints.First(x => x.Endpoint == "/api/a");
        apiA!.CallCount.ShouldBe(2);
        apiA!.AverageExecutionTime.ShouldBe(150);
    }

    [Fact]
    public async Task GetStatisticsAsync_Should_Handle_Empty_Metrics()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetListAsync(false, default)).ReturnsAsync(new List<ApiUsageMetric>());

        // Act
        var stats = await _appService.GetStatisticsAsync();

        // Assert
        stats.ShouldNotBeNull();
        stats!.TotalCalls.ShouldBe(0);
        stats!.MostUsedEndpoints.ShouldBeEmpty();
    }
}
