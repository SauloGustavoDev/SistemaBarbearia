using Api.Modelos.Entidades;
using Api.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel;
namespace Api.Infraestrutura.Contexto
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> option) : base(option)
        {

        }
        public DbSet<Barbeiro> Barbeiro { get; set; }
        public DbSet<Servico> Servico { get; set; }
        public DbSet<Agendamento> Agendamento { get; set; }
        public DbSet<Horario> Horario { get; set; }
        public DbSet<AgendamentoHorario> AgendamentoHorario { get; set; }
        public DbSet<AgendamentoServico> AgendamentoServico { get; set; }
        public DbSet<BarbeiroHorario> BarbeiroHorario { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // BARBEIRO
            modelBuilder.Entity<Barbeiro>(entity =>
            {
                entity.ToTable("barbeiro"); // tabela em minúsculo
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Senha).HasColumnName("senha");
                entity.Property(e => e.Nome).HasColumnName("nome");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.Numero).HasColumnName("numero");
                entity.Property(e => e.Foto).HasColumnName("foto");
                entity.Property(e => e.Acesso).HasColumnName("acesso").HasConversion<int>();
                entity.Property(e => e.Descricao).HasColumnName("descricao");
                entity.Property(e => e.DtCadastro).HasColumnName("dtcadastro");
                entity.Property(e => e.DtDemissao).HasColumnName("dtdemissao");

                entity.HasMany(e => e.Servicos)
                      .WithOne()
                      .HasForeignKey("barbeiro_id"); // FK na tabela servico

                entity.HasMany(e => e.BarbeiroHorario)
                      .WithOne()
                      .HasForeignKey("barbeiro_id");

                entity.HasMany(e => e.Agendamentos)
                      .WithOne()
                      .HasForeignKey("barbeiro_id");
            });

            // SERVICO
            modelBuilder.Entity<Servico>(entity =>
            {
                entity.ToTable("servico");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Descricao).HasColumnName("descricao");
                entity.Property(e => e.Valor).HasColumnName("valor");
                entity.Property(e => e.TempoEstimado).HasColumnName("tempoestimado");
                entity.Property(e => e.DtInicio).HasColumnName("dtinicio");
                entity.Property(e => e.DtFim).HasColumnName("dtfim");
            });

            // BARBEIROHORARIO
            modelBuilder.Entity<BarbeiroHorario>(entity =>
            {
                entity.ToTable("barbeirohorario");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Hora).HasColumnName("hora");
                entity.Property(e => e.DiaSemana).HasColumnName("diasemana");
                entity.Property(e => e.DtInicio).HasColumnName("dtinicio");
                entity.Property(e => e.DtFim).HasColumnName("dtfim");
            });

            modelBuilder.Entity<AgendamentoHorario>(entity =>
            {
                entity.ToTable("agendamentohorario");  // nome da tabela tudo minúsculo
                entity.HasKey(e => e.Id);

                // Relacionamento com Horario
                entity.HasOne(e => e.Horario)
                      .WithMany()                  // se Horario não tiver coleção de AgendamentoHorario
                      .HasForeignKey(e => e.HorarioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Horario>(entity =>
            {
                entity.ToTable("horario");  // tabela tudo minúsculo
                entity.HasKey(e => e.Id);

                // Converter TimeOnly para TimeSpan para PostgreSQL
                entity.Property(e => e.Hora)
                      .HasColumnType("time")
                      .HasConversion(
                          v => v.ToTimeSpan(),    // para armazenar
                          v => TimeOnly.FromTimeSpan(v) // ao ler
                      );
            });
        }
    }
}