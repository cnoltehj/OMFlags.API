using OMFlags.Domain.Entities;

namespace OMFlags.Domain.Contracts;

public interface ICountryService
{
    Task<IReadOnlyList<Country>> GetCountries(CancellationToken ct = default);
    Task<CountryDetails?> GetByNameAsync(string name, CancellationToken ct = default);
}
