using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class WhatsappController : GsSystemControllerBase
    {
        private readonly IConfiguration _config;

        public WhatsappController(IConfiguration configuration)
        {
            _config = configuration;

        }
        [HttpGet("endpoint")]
        public IActionResult Verificar(
          [FromQuery(Name = "hub.mode")] string mode,
          [FromQuery(Name = "hub.challenge")] string challenge,
          [FromQuery(Name = "hub.verify_token")] string token)
        {
            var verifyToken = _config.GetSection("WhatsappApi:Verify_Token").Value;

            if (mode == "subscribe" && token == verifyToken)
            {
                Console.WriteLine("WEBHOOK VERIFICADO");
                return Content(challenge, "text/plain"); // CORRETO: texto puro
            }

            return Forbid();
        }
    }
}
