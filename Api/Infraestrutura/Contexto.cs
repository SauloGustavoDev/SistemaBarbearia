using Api.Modelos.Entidades;
using Api.Modelos.Enums;
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
        public DbSet<AgendamentoHorario> AgendamentoHorario { get; set; }
        public DbSet<AgendamentoServico> AgendamentoServico { get; set; }
        public DbSet<BarbeiroHorario> BarbeiroHorario { get; set; }
        public DbSet<BarbeiroHorarioExcecao> BarbeiroHorarioExcecao { get; set; }
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
            });

            // Api.Infraestrutura.Contexto/Contexto.cs
            modelBuilder.Entity<Servico>(entity =>
            {
                entity.ToTable("servico");
                entity.HasKey(e => e.Id);

                // 🔴 Adicionar este mapeamento para a PK
                entity.Property(e => e.Id).HasColumnName("id");

                // Mapear o enum para ser salvo como string ou int no banco
                entity.Property(e => e.Descricao)
                      .HasColumnName("descricao")
                      .HasConversion<string>(); // Salva o nome do enum (ex: "CorteCabelo")

                entity.Property(e => e.Valor).HasColumnName("valor");

                // 🔴 Mapear TimeOnly explicitamente para 'time'
                entity.Property(e => e.TempoEstimado)
                      .HasColumnName("tempoestimado")
                      .HasColumnType("time");

                entity.Property(e => e.DtInicio).HasColumnName("dtinicio");
                entity.Property(e => e.DtFim).HasColumnName("dtfim");
            });

            // Api.Infraestrutura.Contexto/Contexto.cs
            modelBuilder.Entity<BarbeiroHorario>(entity =>
            {
                entity.ToTable("barbeirohorario");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");

                // 🟢 Mapear a nova propriedade FK
                entity.Property(e => e.IdBarbeiro).HasColumnName("idbarbeiro");

                entity.Property(e => e.Hora).HasColumnName("hora").HasColumnType("time");
                entity.Property(e => e.TipoDia).HasColumnName("tipodia").HasConversion<string>();
                entity.Property(e => e.DtInicio).HasColumnName("dtinicio");
                entity.Property(e => e.DtFim).HasColumnName("dtfim");

                // 🟢 Ajustar o relacionamento para usar a FK explícita
                entity.HasOne<Barbeiro>()
                    .WithMany(b => b.BarbeiroHorario)
                    .HasForeignKey(bh => bh.IdBarbeiro); // Usar a propriedade IdBarbeiro
            });

            // BARBEIROHORARIOEXCECAO
            modelBuilder.Entity<BarbeiroHorarioExcecao>(entity =>
            {
                entity.ToTable("barbeirohorarioexcecao");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.MotivoExcecao).HasColumnName("motivoexcecao");
                entity.Property(e => e.DtExcecao).HasColumnName("dtexcecao");
                entity.Property(e => e.BarbeiroHorarioId).HasColumnName("idbarbeirohorario");

                entity.HasOne(e => e.BarbeiroHorario)
                    .WithOne(h => h.BarbeiroHorarioExcecao)
                    .HasForeignKey<BarbeiroHorarioExcecao>(e => e.BarbeiroHorarioId) // 🔴 FK no lado da exceção
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Agendamento>(entity =>
            {
                entity.ToTable("agendamento");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");

                // Mapeamento explícito da propriedade para a coluna
                entity.Property(e => e.IdBarbeiro).HasColumnName("idbarbeiro");

                entity.Property(e => e.NumeroCliente).HasColumnName("numerocliente");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.NomeCliente).HasColumnName("nomecliente");
                entity.Property(e => e.DtAgendamento).HasColumnName("dtagendamento");

                // Configuração do relacionamento
                entity.HasOne(a => a.Barbeiro)          // Um agendamento tem um barbeiro
                      .WithMany(b => b.Agendamento)   // Um barbeiro tem muitos agendamentos
                      .HasForeignKey(a => a.IdBarbeiro) // A chave estrangeira é IdBarbeiro
                      .HasConstraintName("fk_agendamento_barbeiro"); // Nome da constraint (opcional, mas boa prática)
            });
            modelBuilder.Entity<AgendamentoHorario>(entity =>
            {
                entity.ToTable("agendamentohorario");
                entity.HasKey(e => e.Id);

                // Adicione esta linha para mapear a chave primária
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IdAgendamento).HasColumnName("idagendamento");
                entity.Property(e => e.IdBarbeiroHorario).HasColumnName("idbarbeirohorario");

                entity.HasOne(e => e.Agendamento)
                      .WithMany(a => a.AgendamentoHorarios)
                      .HasForeignKey(e => e.IdAgendamento)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.BarbeiroHorario)
                      .WithMany()
                      .HasForeignKey(e => e.IdBarbeiroHorario)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Api.Infraestrutura.Contexto/Contexto.cs
            modelBuilder.Entity<AgendamentoServico>(entity =>
            {
                entity.ToTable("agendamentoservico");
                // Chave primária composta
                entity.HasKey(e => new { e.IdAgendamento, e.IdServico });

                entity.Property(e => e.IdAgendamento).HasColumnName("idagendamento");
                entity.Property(e => e.IdServico).HasColumnName("idservico");

                // Relacionamento com Agendamento
                entity.HasOne(e => e.Agendamento)
                      .WithMany(a => a.AgendamentoServicos)
                      .HasForeignKey(e => e.IdAgendamento);

                // Relacionamento com Servico
                entity.HasOne(e => e.Servico)
                      .WithMany()
                      .HasForeignKey(e => e.IdServico);
            });

        }
    }
}