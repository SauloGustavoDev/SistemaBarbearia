using Api.Aplicacao.Helpers;
using System.Text.Json.Serialization;

namespace Api.Modelos.Whatsapp
{
    public class Template
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("language")]
        public Language? Language { get; set; }
        [JsonPropertyName("components")]
        public List<Component>? Components { get; set; }
    }
}
