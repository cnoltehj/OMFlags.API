namespace OMFlags.API.Models.Common
{
    public class AppSettings
    {
        public LocalUrl LocalUrl { get; set; }
        public DownstreamUrl DownstreamUrl { get; set; }
        public UpstreamUrl UpstreamUrl { get; set; }
    }

    public class LocalUrl
    {
        public string BaseUrl { get; set; }
    }

    public class DownstreamUrl
    {
        public string BaseUrl { get; set; }
        public string AllCountriesUrl { get; set; }
        public string CountriesFlagsUrl { get; set; }
        public string CountriesNameListUrl { get; set; }
    }

    public class UpstreamUrl
    {
        public string BaseUrlSSl { get; set; }
        public string BaseUrl { get; set; }
    }
}
