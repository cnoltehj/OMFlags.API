using MediatR;
using OMFlags.Domain.Contracts;
using OMFlags.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMFlags.Application.Countries.Handlers
{
    public class GetCountryByNameQueryHandler: IRequestHandler<GetCountryByNameQuery, CountryDetails?>
    {
        private readonly ICountryService _countryService; 

        public GetCountryByNameQueryHandler(ICountryService countryService)
            => _countryService = countryService;

        public async Task<CountryDetails?> Handle(GetCountryByNameQuery request,CancellationToken ct)
        {
          
            if (string.IsNullOrWhiteSpace(request.Name)) return null;

            return await _countryService.GetByNameAsync(request.Name, ct);
        }
    }
}
