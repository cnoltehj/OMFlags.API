using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMFlags.Domain.Entities
{
    public sealed class RawCountryDetails
    {
        public RawName? name { get; set; }
        public List<string>? capital { get; set; }
        public long? population { get; set; }
        public double? area { get; set; }
        public Dictionary<string, string>? languages { get; set; }
        public Dictionary<string, RawCurrency>? currencies { get; set; }
        public List<string>? timezones { get; set; }
        public RawMaps? maps { get; set; }
        public RawFlags? flags { get; set; }
    }

    public sealed class RawName
    {
        public string? common { get; set; }
        public string? official { get; set; }
    }

}
