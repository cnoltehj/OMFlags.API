using MediatR;
using OMFlags.Domain.Contracts;
using OMFlags.Domain.Entities;

namespace OMFlags.Application.Countries;

public record GetCountriesQuery() : IRequest<IReadOnlyList<Country>>;

public class GetCountriesQueryHandler : IRequestHandler<GetCountriesQuery, IReadOnlyList<Country>>
{
    private readonly ICountryService _service;
    public GetCountriesQueryHandler(ICountryService service) => _service = service;

    public async Task<IReadOnlyList<Country>> Handle(GetCountriesQuery request, CancellationToken ct)
    {
        return await _service.GetCountries(ct);
    }
}
