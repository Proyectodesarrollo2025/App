using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Identity;
using Volo.Abp.Account;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using FAFS.Destinations;
using FAFS.Application.Contracts.Destinations;
using Moq;

namespace FAFS;

[DependsOn(
    typeof(FAFSApplicationModule),
    typeof(FAFSDomainTestModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule)
)]
public class FAFSApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var mockCitySearchService = new Moq.Mock<FAFS.Destinations.ICitySearchService>();
        
        // Configuración básica del Mock
        mockCitySearchService.Setup(s => s.SearchCitiesAsync(Moq.It.IsAny<FAFS.Application.Contracts.Destinations.CitySearchRequestDto>()))
            .ReturnsAsync(new FAFS.Application.Contracts.Destinations.CitySearchResultDto
            {
                Cities = new System.Collections.Generic.List<FAFS.Application.Contracts.Destinations.CityDto>
                {
                    new FAFS.Application.Contracts.Destinations.CityDto { Id = "test1", Name = "Test City", Country = "Test Country" }
                }
            });

        mockCitySearchService.Setup(s => s.GetCityDetailsAsync(Moq.It.IsAny<string>()))
            .ReturnsAsync(new FAFS.Application.Contracts.Destinations.CityDto { Id = "test1", Name = "Test City", Country = "Test Country", Population = 100000 });

        context.Services.AddSingleton(mockCitySearchService.Object);
    }
}
