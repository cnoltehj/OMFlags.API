# OMFlags
This respository include both the UI (Blazor) and API (RestFul) DotNet Core
# OMFlags.API

A Clean-Architecture flavored ASP.NET Core Web API that exposes "countries" data to client apps (e.g., a Blazor WASM UI).  
The API aggregates country info (via an adapter over RestCountries) and serves it from "two endpoints" used by the frontend:

- "GET /api/Countries/GetCountries" — list for the flags grid
- "GET /api/Countries/GetCountryDetailsAsync?name=South%20Africa" — full country details for the modal

---

## Table of Contents
- [Features](features)
- [Architecture](architecture)
- [Requirements](requirements)
- [Local Run (no Docker)](local-run-no-docker)
- [Docker Run](docker-run)
- [Configuration](configuration)
  - [CORS](cors)
  - [HTTPS Certificates (dev)](https-certificates-dev)
  - [Typed HttpClients](typed-httpclients)
- [API Endpoints](api-endpoints)
- [Logging](logging)
- [Health & Swagger](health--swagger)
- [Troubleshooting](troubleshooting)
- [Project Structure](project-structure)
- [cURL Quick Reference](curl-quick-reference)

---

## Features
- "Clean layering": API (controllers) → MediatR → Application (handlers/DTOs) → Infrastructure (adapters/HttpClient)
- "CORS-safe" for a Blazor dev UI at  "https://localhost:7245"
- "HTTP/HTTPS" hosting for local dev ("http://localhost:7020", "https://localhost:7021")
- "Swagger/OpenAPI" enabled
- "NLog" integrated with "ILogger<T>" for structured logging

---

## Architecture

"""
OMFlags.API (ASP.NET Core)
  ├─ Controllers (e.g., CountriesController) -> IMediator
  ├─ Startup/Program (CORS, Swagger, DI, Logging)
  └─ Uses Application & Infrastructure layers
OMFlags.Application
  ├─ DTOs (CountryDto, CountryDetailDto, etc.)
  ├─ Interfaces (ICountryService / ICountryApi)
  └─ MediatR Requests & Handlers (GetCountriesQuery, GetCountryDetailsQuery)
OMFlags.Infrastructure
  └─ Adapters/Services (HttpClient to RestCountries, mapping, resilience)
"""

"Controller flow:"  
"CountriesController" → "IMediator.Send(query)" → handler uses "ICountryService" → adapter fetches from RestCountries, maps to DTOs → returns to controller.

---

## Requirements
- .NET 8 SDK
- (Optional) Docker Desktop (if using Docker)
- Dev HTTPS certificate (for "https://localhost:7021")

---

## Local Run (no Docker)

"""bash
cd OMFlags.API
dotnet restore
dotnet run
"""
Default dev URLs (Kestrel):

- HTTP:  "http://localhost:7020"  
- HTTPS: "https://localhost:7021"

Open Swagger:

- "http://localhost:7020/swagger"  
- "https://localhost:7021/swagger"

---

## Docker Run

"docker-compose.yml" (service: ""omflags.api"") exposes:

- HTTP:  "http://localhost:7020"  
- HTTPS: "https://localhost:7021"

""One-time dev cert setup (host):""
"""bash
dotnet dev-certs https --clean
dotnet dev-certs https -ep ~/.aspnet/https/aspnetapp.pfx -p SuperSecret123
dotnet dev-certs https --trust

# expose password for docker compose
# PowerShell
$Env:ASPNETCORE_HTTPS_PASSWORD="SuperSecret123"
# bash/zsh
export ASPNETCORE_HTTPS_PASSWORD=SuperSecret123
"""

""Start:""
"""bash
docker compose up --build
"""

""Swagger in Docker:""

- "http://localhost:7020/swagger"  
- "https://localhost:7021/swagger"

---

## Configuration

### CORS

Startup/Program should allow your UI origin(s), e.g. "https://localhost:7245":

"""csharp
const string CorsPolicyName = "FrontendPolicy";

builder.Services.AddCors(o =>
{
    o.AddPolicy(CorsPolicyName, p => p
        .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                     ?? new[] { "https://localhost:7245" })
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

app.UseCors(CorsPolicyName);
app.MapControllers().RequireCors(CorsPolicyName);
"""

Environment (e.g., Docker):
"""
Cors__AllowedOrigins = "https://localhost:7245,http://localhost:7245"
"""

### HTTPS Certificates (dev)

Kestrel reads a mounted PFX:

"""
ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ASPNETCORE_Kestrel__Certificates__Default__Password=<your_password>
"""

### Typed HttpClients

Outbound call to RestCountries (inside API):

"""csharp
services.AddHttpClient<ICountryService, CountryService>(c =>
{
    c.BaseAddress = new Uri("https://restcountries.com/v3.1/");
    c.Timeout = TimeSpan.FromSeconds(30);
});
"""

Optional generic client:

"""csharp
services.AddHttpClient<IApiClientAsync, ApiClientAsync>(c =>
{
    var baseUrl = Configuration["Backend:BaseUrl"] ?? "https://localhost:7021/";
    c.BaseAddress = new Uri(baseUrl);
});
"""

---

## API Endpoints

### 1) List Countries (for flags grid)

"GET /api/Countries/GetCountries"

""Response (example):""
"""json
[
  { "id": 1, "code": "", "name": "Afghanistan",
    "flagUrl": "https://upload.wikimedia.org/...png" },
  { "id": 2, "code": "", "name": "Åland Islands",
    "flagUrl": "https://flagcdn.com/w320/ax.png" }
]
"""

### 2) Country Details (for modal)

"GET /api/Countries/GetCountryDetailsAsync?name=South%20Africa"

""Response (example):""
"""json
{
  "name": "South Africa",
  "flagPng": null,
  "capital": "Pretoria",
  "population": 59308690,
  "area": 1221037,
  "timezones": ["UTC+02:00"],
  "languages": ["Afrikaans","English","..."],
  "currencies": [{ "code": "ZAR", "name": "South African rand", "symbol": "R" }],
  "googleMaps": "https://goo.gl/maps/...",
  "openStreetMaps": "https://www.openstreetmap.org/relation/87565"
}
"""

---

## Logging

### API (NLog + "ILogger<T>")

Bootstrapped in "Program.cs":

"""csharp
var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
"""

Usage:

"""csharp
public class CountriesController : ControllerBase
{
    private readonly ILogger<CountriesController> _log;
    private readonly IMediator _mediator;

    public CountriesController(ILogger<CountriesController> log, IMediator mediator)
    {
        _log = log; _mediator = mediator;
    }

    [HttpGet("GetCountries")]
    public async Task<IActionResult> GetCountries(CancellationToken ct)
    {
        _log.LogInformation("Fetching countries");
        var list = await _mediator.Send(new GetCountriesQuery(), ct);
        _log.LogInformation("Fetched {Count} countries", list.Count);
        return Ok(list);
    }
}
"""

Configure targets in "nlog.config" (console + rolling file).  
Control log levels via "appsettings.Development.json" or "nlog.config".

---

## Health & Swagger

- Swagger: "http(s)://localhost:7020|7021/swagger"
- Optional Health Checks:

"""csharp
services.AddHealthChecks(); // + AddSqlServer(...)
app.MapHealthChecks("/health");
"""

---

## Troubleshooting

- ""CORS blocked"": Ensure Blazor origin matches exactly; call "UseCors" before "MapControllers".
- ""IMediator not resolved"":
  - Install MediatR in API & Application.
  - "services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetCountriesQuery).Assembly));"
  - Ensure API references Application.
- ""Flags not shown in UI"":
  - UI must bind to "flagUrl".
  - If image URLs are "http://", normalize to "https://" in the adapter or proxy via API.
- ""Modal renders at page bottom"" (UI): Use modal with "position: fixed; z-index" and render details only inside the modal.

---

## Project Structure

"""
OMFlags.API/
  Controllers/
    CountriesController.cs
  Program.cs / Startup.cs
  nlog.config
OMFlags.Application/
  Countries/
    Queries/
      GetCountriesQuery.cs
      GetCountryDetailsQuery.cs
    DTOs/
      CountryDto.cs
      CountryDetailDto.cs
  Abstractions/Interfaces (ICountryService/ICountryApi)
OMFlags.Infrastructure/
  Adapters/
    CountryApiAdapter.cs
  Services/
    CountryService.cs
"""

---

## cURL Quick Reference

""List:""
"""bash
curl -X GET "https://localhost:7021/api/Countries/GetCountries" -H "accept: application/json"
"""

""Details:""
"""bash
curl -X GET "https://localhost:7021/api/Countries/GetCountryDetailsAsync?name=South%20Africa" -H "accept: application/json"
"""
