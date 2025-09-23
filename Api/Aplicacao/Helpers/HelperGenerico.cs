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
    }
}
