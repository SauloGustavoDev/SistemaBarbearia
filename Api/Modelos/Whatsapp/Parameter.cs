using System.Text.Json.Serialization;

namespace Api.Modelos.Whatsapp
{
    public class Parameter
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
