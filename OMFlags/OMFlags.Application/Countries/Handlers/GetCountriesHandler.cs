using MediatR;
using OMFlags.Application.Countries;
using OMFlags.Domain.Contracts;
using OMFlags.Domain.Entities;

public class GetCountriesHandler : IRequestHandler<GetCountriesQuery, IReadOnlyList<Country>>
{
    private readonly ICountryService _svc;
    public GetCountriesHandler(ICountryService svc) => _svc = svc;

    public Task<IReadOnlyList<Country>> Handle(GetCountriesQuery request, CancellationToken ct)
        => _svc.GetCountries(ct);
}
