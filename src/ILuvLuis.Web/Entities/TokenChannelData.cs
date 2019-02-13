using Newtonsoft.Json;

namespace ILuvLuis.Web.Entities
{
    public class TokenChannelData
    {
        [JsonProperty("qavantToken")]
        public string QavantToken { get; set; }
    }
}
