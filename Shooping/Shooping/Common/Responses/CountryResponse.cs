using Newtonsoft.Json;

namespace Shooping.Common.Responses
{
    public class CountryResponse
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("iso2")]
        public string Iso2 { get; set; }
    }
}
