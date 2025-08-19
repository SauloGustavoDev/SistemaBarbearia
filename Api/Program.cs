using Api.Aplicacao.Contratos;
using Api.Aplicacao.Servicos;
using Api.Infraestrutura.Contexto;
using Microsoft.EntityFrameworkCore;
using SuaEmpresa.SuaApp.Infraestrutura.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<Contexto>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgresDb")
    )
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBarbeiroApp, BarbeiroApp>();
builder.Services.AddScoped<IAgendamentoApp, AgendamentoApp>();

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MinhaPoliticaCors", policy =>
    {
        policy.WithOrigins(allowedOrigins ?? Array.Empty<string>())
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("MinhaPoliticaCors");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
