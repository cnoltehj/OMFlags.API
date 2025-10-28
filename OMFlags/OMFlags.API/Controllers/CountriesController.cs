using MediatR;
using Microsoft.AspNetCore.Mvc;
using OMFlags.API.Mappers;
using OMFlags.API.Models;
using OMFlags.Application.Countries;
using OMFlags.Domain.Contracts;
using OMFlags.Domain.Entities;

namespace OMFlags.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ILogger<CountriesController> _logger;
        private readonly IApiClientAsync _apiClientAsync;
        private readonly IMediator _mediator;
        public CountriesController(ILogger<CountriesController> logger , IApiClientAsync apiClientAsync, IMediator mediator) 
        {
            _logger = logger;
            _apiClientAsync = apiClientAsync;
            _mediator = mediator;
        }


        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<Country>>> GetCountriesAsync()
        {
            var countries = await _mediator.Send(new GetCountriesQuery());
            return Ok(countries);
        }

        // /nameof(GetCountryDetailsAsync), Name = "{name}/details"
        [HttpGet(nameof(GetCountryDetailsAsync), Name = "{name}/details")]
        public async Task<ActionResult<CountryDetails>> GetCountryDetailsAsync(string name)
        {
            var details = await _mediator.Send(new GetCountryByNameQuery(name));
            if (details is null) return NotFound();
            return Ok(details);
        }
    }
}
