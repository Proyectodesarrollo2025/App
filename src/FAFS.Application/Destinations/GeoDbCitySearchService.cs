using FAFS.Application.Contracts.Destinations;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Volo.Abp;

namespace FAFS.Destinations
{
    // Service that connects to the external GeoDB Cities API
    public class GeoDbCitySearchService : ICitySearchService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ExternalApisOptions _options;

        public GeoDbCitySearchService(
            IHttpClientFactory httpClientFactory,
            IOptions<ExternalApisOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

        public async Task<CitySearchResultDto> SearchCitiesAsync(CitySearchRequestDto request)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(request.PartialName) || request.PartialName.Trim().Length < 2)
            {
                throw new BusinessException("CitySearch:InvalidPartialName")
                    .WithData("Message", "The search text must contain at least 2 characters.");
            }

            // Crear cliente HTTP
            var client = _httpClientFactory.CreateClient("GeoDbClient");
            client.BaseAddress = new Uri(_options.GeoDb.BaseUrl);
            client.DefaultRequestHeaders.Add("X-RapidAPI-Key", _options.GeoDb.ApiKey);
            client.DefaultRequestHeaders.Add("X-RapidAPI-Host", _options.GeoDb.ApiHost);

            // Construir la URL del request
            var url = $"cities?namePrefix={Uri.EscapeDataString(request.PartialName)}&limit={request.Limit}";

            if (!string.IsNullOrWhiteSpace(request.CountryCode))
            {
                url += $"&countryIds={Uri.EscapeDataString(request.CountryCode)}";
            }

            // Mostrar en consola la URL completa que se va a llamar
            var fullUrl = $"{client.BaseAddress}{url}";
            Console.WriteLine($"[DEBUG] Request URL final: {fullUrl}");

            try
            {
                // Llamar al endpoint externo
                var response = await client.GetAsync(url);

                // Si la respuesta no fue exitosa, mostrar detalles
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ERROR] Status: {(int)response.StatusCode} - {response.ReasonPhrase}");
                    Console.WriteLine($"[ERROR] Body: {errorContent}");
                }
               

                // Por esta línea correcta:
                Console.WriteLine($"URL final: {_options.GeoDb.BaseUrl}{url}");
                // Asegurar éxito (lanzará excepción si no es 2xx)
                response.EnsureSuccessStatusCode();

                using var stream = await response.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);

                // Procesar la respuesta JSON
                var result = new CitySearchResultDto();

                if (doc.RootElement.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in data.EnumerateArray())
                    {
                        result.Cities.Add(new CityDto
                        {
                            Name = item.GetProperty("name").GetString() ?? string.Empty,
                            Country = item.GetProperty("country").GetString() ?? string.Empty,
                            CountryCode = item.GetProperty("countryCode").GetString() ?? string.Empty,
                            Region = item.TryGetProperty("region", out var r) ? r.GetString() : null,
                            Latitude = item.TryGetProperty("latitude", out var lat) ? lat.ToString() : null,
                            Longitude = item.TryGetProperty("longitude", out var lon) ? lon.ToString() : null
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION] {ex.Message}");
                throw;
            }
        }
    }

    // External API configuration 
    public class ExternalApisOptions
    {
        public GeoDbOptions GeoDb { get; set; } = new();

        public class GeoDbOptions
        {
            public string BaseUrl { get; set; } = "https://wft-geo-db.p.rapidapi.com/v1/geo";
            public string ApiHost { get; set; } = "wft-geo-db.p.rapidapi.com";
            public string ApiKey { get; set; } = string.Empty;
        }
    }
}