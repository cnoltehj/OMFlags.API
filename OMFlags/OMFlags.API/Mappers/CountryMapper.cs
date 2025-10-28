using OMFlags.Domain.Entities;

namespace OMFlags.API.Mappers
{
    public static class CountryMapper
    {
        private static readonly ILogger? _log;

        public static CountryDetails Mapper(this RawCountryDetails rc)
        {

            if (rc is null)
            {
                _log?.LogWarning("RawCountryDetails is null. Returning empty CountryDetails.");
                return new CountryDetails();
            }

            try { 
            

            var countrydetail = new CountryDetails
            {
                Name = rc.name?.common ?? rc.name?.official ?? "Unknown",
                FlagPng = rc.flags?.png,
                Capital = rc.capital?.FirstOrDefault(),
                Population = rc.population,
                Area = rc.area,
                GoogleMaps = rc.maps?.googleMaps,
                OpenStreetMaps = rc.maps?.openStreetMaps,
                Timezones = rc.timezones ?? new List<string>(),
                Languages = rc.languages?.Values.ToList() ?? new List<string>()
            };

            if (rc.currencies is not null)
            {
                foreach (var kv in rc.currencies)
                {
                    var code = kv.Key;
                    var cur = kv.Value;
                    countrydetail.Currencies.Add(new Currency
                    {
                        Code = code,
                        Name = cur.name ?? "",
                        Symbol = cur.symbol
                    });
                }
             }

             return countrydetail;

            }
            catch (Exception ex)
            {
                _log?.LogError(ex, "Error while mapping RawCountryDetails to CountryDetails.");
            }

            return null;
        }

    }
}
