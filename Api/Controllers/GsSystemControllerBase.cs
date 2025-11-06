using Microsoft.AspNetCore.Mvc;


namespace Api.Controllers
{
    public class GsSystemControllerBase : ControllerBase
    {
        protected int GetUserId()
        {
            var id = User.Claims?.FirstOrDefault(a => a.Type == "Id")?.Value;
            return id == null ? throw new Exception("Não autorizado") : int.Parse(id);
        }
        protected string GetUserNumero()
        {
            var numero = User.Claims?.FirstOrDefault(a => a.Type == "Numero")?.Value;
            if (numero == null)
                throw new Exception("Não autorizado");

            return numero;
        }
    }
}
