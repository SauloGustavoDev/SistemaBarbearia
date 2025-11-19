using Api.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;
namespace Api.Infraestrutura.Contexto
{
    public class Contexto(DbContextOptions<Contexto> option) : DbContext(option)
    {
        public DbSet<Barbeiro> Barbeiro { get; set; }
        public DbSet<Mensalista> Mensalista { get; set; }
        public DbSet<MensalistaDia> MensalistaDia { get; set; }
        public DbSet<Servico> Servico { get; set; }
        public DbSet<Agendamento> Agendamento { get; set; }
        public DbSet<AgendamentoHorario> AgendamentoHorario { get; set; }
        public DbSet<AgendamentoServico> AgendamentoServico { get; set; }
        public DbSet<BarbeiroHorario> BarbeiroHorario { get; set; }
        public DbSet<BarbeiroHorarioExcecao> BarbeiroHorarioExcecao { get; set; }
        public DbSet<CategoriaServico> CategoriaServico { get; set; }
        public DbSet<BarbeiroServico> BarbeiroServico { get; set; }
        public DbSet<CodigoConfirmacao> TokenConfirmacao { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MensalistaDia>(entity =>
            {
                entity.ToTable("mensalistadia");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DiaSemana).HasColumnName("DiaSemana").HasConversion<string>();
                entity.Property(e => e.Horario).HasColumnName("Horario").HasColumnType("time");
            });

            modelBuilder.Entity<Mensalista>(entity =>
            {
                entity.ToTable("mensalista");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Nome).HasColumnName("nome").IsRequired();
                entity.Property(e => e.Valor).HasColumnName("valor").IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Numero).HasColumnName("numero").IsRequired();
                entity.Property(e => e.IdBarbeiro).HasColumnName("idbarbeiro");
                entity.Property(e => e.Tipo).HasColumnName("tipo").HasConversion<string>(); // Converte o enum para string no banco
                entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>(); // Converte o enum para string no banco

                entity.Property(e => e.DtInicio).HasColumnName("dtinicio");
                entity.Property(e => e.DtFim).HasColumnName("dtfim");

                entity.HasOne<Barbeiro>()
                .WithMany(b => b.Mensalistas)
                      .HasForeignKey(m => m.IdBarbeiro)
                      .OnDelete(DeleteBehavior.Restrict); // Impede apagar barbeiro com mensalistas ativos
            });

            modelBuilder.Entity<CodigoConfirmacao>(entity =>
            {
                entity.ToTable("tokenconfirmacao");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Numero).HasColumnName("numero").IsRequired();
                entity.Property(e => e.Codigo).HasColumnName("codigo").IsRequired();
                entity.Property(e => e.DtCriacao).HasColumnName("dtcriacao").IsRequired();
                entity.Property(e => e.DtExpiracao).HasColumnName("dtexpiracao");
                entity.Property(e => e.Confirmado).HasColumnName("confirmado").IsRequired();
            });

            modelBuilder.Entity<CategoriaServico>(entity =>
            {
                entity.ToTable("categoriaservico");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Descricao)
                      .HasColumnName("descricao")
                      .IsRequired();
                entity.Property(e => e.DtInicio).HasColumnName("dtinicio");
                entity.Property(e => e.DtFim).HasColumnName("dtfim");
                entity.HasMany(c => c.Servicos)
                      .WithOne(s => s.CategoriaServico)
                      .HasForeignKey(s => s.IdCategoriaServico)
                      .OnDelete(DeleteBehavior.Restrict); // Impede apagar uma categoria que ainda tenha serviços associados.
            });


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
                entity.Property(e => e.Acesso).HasColumnName("acesso").HasConversion<string>();
                entity.Property(e => e.Agenda).HasColumnName("tipoagenda").HasConversion<string>();
                entity.Property(e => e.Descricao).HasColumnName("descricao");
                entity.Property(e => e.DtCadastro).HasColumnName("dtcadastro");
                entity.Property(e => e.DtDemissao).HasColumnName("dtdemissao");
            });

            modelBuilder.Entity<Servico>(entity =>
            {
                entity.ToTable("servico");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Descricao).HasColumnName("nome").IsRequired(); // Agora é 'Nome'
                entity.Property(e => e.Valor).HasColumnName("valor").HasColumnType("decimal(18,2)"); // Boa prática especificar precisão
                entity.Property(e => e.TempoEstimado).HasColumnName("tempoestimado").HasColumnType("time");
                entity.Property(e => e.DtInicio).HasColumnName("dtinicio");
                entity.Property(e => e.DtFim).HasColumnName("dtfim");
                entity.Property(e => e.IdCategoriaServico).HasColumnName("idcategoriaservico");
                entity.HasOne(s => s.CategoriaServico)
                      .WithMany(c => c.Servicos)
                      .HasForeignKey(s => s.IdCategoriaServico)
                      .OnDelete(DeleteBehavior.Restrict); // Impede apagar categoria com serviços
            });

            modelBuilder.Entity<BarbeiroServico>(entity =>
            {
                entity.ToTable("barbeiroservico");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.DtInicio).HasColumnName("dtinicio");
                entity.Property(e => e.DtFim).HasColumnName("dtfim");

                entity.Property(e => e.IdBarbeiro).HasColumnName("idbarbeiro");
                entity.HasOne(bs => bs.Barbeiro)
                      .WithMany(b => b.BarbeiroServicos) // 'BarbeiroServicos' é a List<> na entidade Barbeiro
                      .HasForeignKey(bs => bs.IdBarbeiro);
                entity.Property(e => e.IdServico).HasColumnName("idservico");
                entity.HasOne(bs => bs.Servico)
                      .WithMany() // Se Servico não tiver uma lista de BarbeiroServicos, use .WithMany() vazio
                      .HasForeignKey(bs => bs.IdServico);
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
                    entity.HasOne<Barbeiro>()
                        .WithMany(b => b.BarbeiroHorarios)
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
                entity.Property(e => e.IdBarbeiro).HasColumnName("idbarbeiro");

                entity.Property(e => e.MetodoPagamento)
                .HasColumnName("metodopagamento") // Define o nome da coluna
                .HasConversion<string>(); // <<-- A MÁGICA ACONTECE AQUI

                entity.Property(e => e.NumeroCliente).HasColumnName("numerocliente");
                entity.Property(e => e.Status).HasColumnName("status")
                .HasConversion<string>();
                entity.Property(e => e.NomeCliente).HasColumnName("nomecliente");
                entity.Property(e => e.DtAgendamento).HasColumnName("dtagendamento");

                // Configuração do relacionamento
                entity.HasOne(a => a.Barbeiro)          // Um agendamento tem um barbeiro
                      .WithMany(b => b.Agendamentos)   // Um barbeiro tem muitos agendamentos
                      .HasForeignKey(a => a.IdBarbeiro) // A chave estrangeira é IdBarbeiro
                      .HasConstraintName("fk_agendamento_barbeiro"); // Nome da constraint (opcional, mas boa prática)
            });
            modelBuilder.Entity<AgendamentoHorario>(entity =>
            {
                entity.ToTable("agendamentohorario");
                entity.HasKey(e => e.Id);
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
                entity.HasKey(e => new { e.IdAgendamento, e.IdServico });

                entity.Property(e => e.IdAgendamento).HasColumnName("idagendamento");
                entity.Property(e => e.IdServico).HasColumnName("idservico");
                entity.HasOne(e => e.Agendamento)
                      .WithMany(a => a.AgendamentoServicos)
                      .HasForeignKey(e => e.IdAgendamento);
                entity.HasOne(e => e.Servico) // ✅ CORRETO: Aponta para a entidade Servico
                .WithMany(s => s.AgendamentoServicos) // Navegação inversa em Servico
                .HasForeignKey(e => e.IdServico);
            });

        }
    }
}