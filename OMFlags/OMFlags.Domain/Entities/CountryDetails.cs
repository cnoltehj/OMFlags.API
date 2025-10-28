using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMFlags.Domain.Entities
{
    public class CountryDetails
    {
        public string Name { get; set; } = "";
        public string? FlagPng { get; set; }
        public string? Capital { get; set; }
        public long? Population { get; set; }
        public double? Area { get; set; }
        public List<string> Timezones { get; set; } = new();
        public List<string> Languages { get; set; } = new();
        public List<Currency> Currencies { get; set; } = new();
        public string? GoogleMaps { get; set; }
        public string? OpenStreetMaps { get; set; }
    }

    public class Currency
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Symbol { get; set; }
    }

    public class RawCountryName 
    { 
        public string? common { get; set; } 
        public string? official { get; set; } 
    }

    public class RawFlags 
    { 
        public string? png { get; set; } 
        public string? svg { get; set; } 
    }

    public class RawMaps 
    { 
        public string? googleMaps { get; set; } 
        public string? openStreetMaps { get; set; } 
    }

    public class RawCurrency 
    { 
        public string? name { get; set; } 
        public string? symbol { get; set; } 
    }

}
