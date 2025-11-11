using System.Text.Json.Serialization;

namespace Api.Modelos.Whatsapp
{
    public class Language
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }
}
