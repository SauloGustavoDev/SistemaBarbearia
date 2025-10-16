using Api.Modelos.Enums;

namespace Api.Aplicacao.Helpers
{
    public static class HelperGenerico
    {
        public static List<TimeOnly> MontarHorarioDiaSemana()
        {
            var horarios = new List<TimeOnly>();
            var horaManha = TimeOnly.Parse("10:00");
            horarios.Add(horaManha);
            while (horaManha < TimeOnly.Parse("12:00"))
            {
                horaManha = horaManha.AddMinutes(40);
                horarios.Add(horaManha);
            }
            var horaTarde = TimeOnly.Parse("13:20");
            horarios.Add(horaTarde);
            while (horaTarde < TimeOnly.Parse("20:00"))
            {
                horaTarde = horaTarde.AddMinutes(40);
                horarios.Add(horaTarde);
            }
            return horarios;
        }

        public static List<TimeOnly> MontarHorarioSabado()
        {
            var horarios = new List<TimeOnly>();
            var horaManha = TimeOnly.Parse("09:00");
            horarios.Add(horaManha);
            while (horaManha < TimeOnly.Parse("12:20"))
            {
                horaManha = horaManha.AddMinutes(40);
                horarios.Add(horaManha);
            }
            var horaTarde = TimeOnly.Parse("13:20");
            horarios.Add(horaTarde);
            while (horaTarde < TimeOnly.Parse("19:00"))
            {
                horaTarde = horaTarde.AddMinutes(40);
                horarios.Add(horaTarde);
            }
            return horarios;
        }

        public static int GerarCodigoConfirmacao()
        {
            Random random = new Random();
            return random.Next(1000, 9999); // Gera um número entre 1000 e 9999
        }

        public static int GerarDiasAgenda(TipoAgenda agenda)
        {
            switch (agenda) 
            {
                case TipoAgenda.Diaria: return 1;
                case TipoAgenda.Semanal: return 7 - (int)DateTime.Now.DayOfWeek;
                case TipoAgenda.Quinzenal: return (7 - (int)DateTime.Now.DayOfWeek) + 7;
                case TipoAgenda.Mensal: return 30;
                case TipoAgenda.Fechada: return 0;
                default: throw new NotImplementedException();
            }
        }
    }
}
