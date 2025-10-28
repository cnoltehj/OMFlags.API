using OMFlags.Domain.Contracts;
using OMFlags.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OMFlags.Infrastructure.Services
{
    public sealed class CountryService : ICountryService
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions JsonOpts =
            new(JsonSerializerDefaults.Web) { PropertyNameCaseInsensitive = true };

        public CountryService(HttpClient http) => _http = http;

        public async Task<IReadOnlyList<Country>> GetCountries(CancellationToken ct = default)
        {
            // restcountries: GET /all?fields=name,flags
            using var resp = await _http.GetAsync("all?fields=name,flags", ct);
            resp.EnsureSuccessStatusCode();

            using var stream = await resp.Content.ReadAsStreamAsync(ct);
            var raw = await JsonSerializer.DeserializeAsync<List<RawCountryDetails>>(stream, JsonOpts, ct)
                      ?? new List<RawCountryDetails>();

            // map RawCountry -> Country (Id, Name, Flag)
            var result = raw
                .Where(r => r.name?.common is not null && r.flags?.png is not null)
                .OrderBy(r => r.name!.common!)
                .Select((r, i) => new Country
                {
                    Id = i + 1,
                    Name = r.name!.common!,
                    FlagUrl = r.flags!.png!
                })
                .ToList();

            return result;
        }

        public async Task<CountryDetails?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            // restcountries: GET /name/{name}?fullText=true&fields=name,population,capital,languages,area,timezones,maps,currencies
            var path = $"name/{Uri.EscapeDataString(name)}?fullText=true&fields=name,population,capital,languages,area,timezones,maps,currencies";
            using var resp = await _http.GetAsync(path, ct);
            if (!resp.IsSuccessStatusCode) return null;

            using var stream = await resp.Content.ReadAsStreamAsync(ct);
            var raw = await JsonSerializer.DeserializeAsync<List<RawCountryDetails>>(stream, JsonOpts, ct);
            var first = raw?.FirstOrDefault();
            if (first is null) return null;

            return new CountryDetails
            {
                Name = first.name?.common ?? name,
                Population = first.population ?? 0,
                Capital = first.capital?.FirstOrDefault() ?? string.Empty,
                Languages = first.languages?.Values?.ToList() ?? new List<string>(),
                Area = first.area ?? 0,
                Timezones = first.timezones ?? new List<string>(),
                GoogleMaps = first.maps?.googleMaps,
                OpenStreetMaps = first.maps?.openStreetMaps,
                FlagPng = first.flags?.png,
                Currencies = first.currencies != null? first.currencies
                  .Select(kvp => new Currency
                  {
                      Code = kvp.Key,
                      Name = kvp.Value?.name,
                      Symbol = kvp.Value?.symbol
                  })
                  .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                  .ToList()
                : new List<Currency>()
            };
        }

    }
}
