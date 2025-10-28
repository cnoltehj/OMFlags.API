using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using OMFlags.API; // Program
using System.Net;
using System.Net.Http.Json;
using Xunit;

public class CountriesEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CountriesEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            // Optional: override config for tests here if needed
            // builder.UseEnvironment("Testing");
        });
    }

    [Fact]
    public async Task GetCountries_ShouldReturn200_AndList()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var resp = await client.GetAsync("/api/Countries/GetCountries");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await resp.Content.ReadFromJsonAsync<List<CountryListItem>>();
        json.Should().NotBeNull();
        json!.Should().OnlyContain(x => !string.IsNullOrWhiteSpace(x.Name));
    }

    [Fact]
    public async Task GetCountryDetails_ShouldReturn200_AndDetails()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/api/Countries/GetCountryDetailsAsync?name=South%20Africa");
        resp.EnsureSuccessStatusCode();

        var details = await resp.Content.ReadFromJsonAsync<CountryDetailsVm>();
        details.Should().NotBeNull();
        details!.Name.Should().Be("South Africa");
        details.Population.Should().BeGreaterThan(0);
    }

    // local-only, small read models for assertions
    private sealed record CountryListItem(int Id, string? Code, string Name, string FlagUrl);
    private sealed record CurrencyVm(string? Code, string? Name, string? Symbol);
    private sealed record CountryDetailsVm(string Name, string? FlagPng, string? Capital,
        long Population, long Area, List<string> Timezones, List<string> Languages,
        List<CurrencyVm> Currencies, string? GoogleMaps, string? OpenStreetMaps);
}
