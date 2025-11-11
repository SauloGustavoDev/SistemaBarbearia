using Api.Modelos.Whatsapp;
using System.Text.Json.Serialization;

namespace Api.Aplicacao.Helpers
{
    public class WhatsappEnvio
    {
        [JsonPropertyName("messaging_product")]
        public string? MessagingProduct { get; set; }
        [JsonPropertyName("to")]
        public string? To { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        [JsonPropertyName("template")]
        public Template? Template { get; set; }

            public WhatsappEnvio(string codigo, string numeroEnvio)
            {
                MessagingProduct = "whatsapp";
                To = numeroEnvio;
                Type = "template";
                Template = new Template
                {
                    Name = "whast_cod_confirmation",
                    Language = new Language { Code = "pt_BR" },
                    Components = new List<Component>
                {
                    new Component
                    {
                        Type = "body",
                        Parameters = new List<Parameter>
                        {
                            new Parameter { Type = "text", Text = codigo }
                        }
                    },
                    new Component
                    {
                        Type = "button",
                        SubType = "url",
                        Index = "0",
                        Parameters = new List<Parameter>
                        {
                            new Parameter { Type = "text", Text = codigo }
                        }
                    }
                }
            };
        }
    }
}
