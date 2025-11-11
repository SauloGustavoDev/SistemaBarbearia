using System.Text.Json.Serialization;

namespace Api.Modelos.Whatsapp
{
    public class Component
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("sub_type")]
        public string? SubType { get; set; }
        [JsonPropertyName("index")]
        public string? Index { get; set; }
        [JsonPropertyName("parameters")]
        public List<Parameter>? Parameters { get; set; }
    }
}
