using Api.Aplicacao.Contratos;
using Api.Aplicacao.Servicos;
using Api.Infraestrutura;
using Api.Infraestrutura.Contexto;
using Api.Infraestrutura.Hangfire;
using Api.Infraestrutura.Middleware;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<Contexto>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Api da Barbearia",
        Version = "v1",
        Description = "Documentação da API de agendamento e controle da barbearia."
    });

    o.CustomSchemaIds(id => id.FullName!.Replace("+", "-"));
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter your JWT Token here",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT"
    };
    o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            []
        }
    };
    o.AddSecurityRequirement(securityRequirement);
});
builder.Services.AddScoped<IExcecaoApp, ExcecaoApp>();
builder.Services.AddScoped<IMensalistaApp, MensalistaApp>();
builder.Services.AddScoped<IBarbeiroApp, BarbeiroApp>();
builder.Services.AddScoped<IRelatorioApp, RelatorioApp>();
builder.Services.AddScoped<IAgendamentoApp, AgendamentoApp>();
builder.Services.AddScoped<IAutenticacaoApp, AutenticacaoApp>();
builder.Services.AddScoped<INotificacaoApp, NotificacaoApp>();
builder.Services.AddScoped<IServicoApp, ServicoApp>();
builder.Services.AddScoped<ITestesApp, TestesApp>();
builder.Services.AddTransient<IWorker, Worker>();
builder.Services.AddSingleton<TokenProvider>();

var jwtSecret = builder.Configuration.GetSection("Jwt:Secret").Value
        ?? throw new InvalidOperationException("Jwt:Secret não está configurado.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
            ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
            ClockSkew = TimeSpan.Zero
        };
    });

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MinhaPoliticaCors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configuração do Hangfire
builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"));
    })
    .UseSimpleAssemblyNameTypeSerializer();

});
builder.Services.AddHangfireServer();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("MinhaPoliticaCors");

var hangUser = builder.Configuration.GetSection("Hangfire:Usuario").Value
    ?? throw new InvalidOperationException("Usuario do hangfire não está configurado.");

var hangPass = builder.Configuration.GetSection("Hangfire:Senha").Value
        ?? throw new InvalidOperationException("Senha do hangfire não está configurado.");

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "Automações",
    Authorization = [new HangfireAuthorizationFilter(hangUser, hangPass)]
});

app.UseHangfireRecurringJobs();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
