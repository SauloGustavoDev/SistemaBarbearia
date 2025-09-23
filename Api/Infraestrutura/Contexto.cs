using Api.Modelos.Entidades;
using Microsoft.EntityFrameworkCore;
namespace Api.Infraestrutura.Contexto
{
    public class Contexto(DbContextOptions<Contexto> option) : DbContext(option)
    {
        public DbSet<Barbeiro> Barbeiro { get; set; }
        public DbSet<Servico> Servico { get; set; }
        public DbSet<Agendamento> Agendamento { get; set; }
        public DbSet<AgendamentoHorario> AgendamentoHorario { get; set; }
        public DbSet<AgendamentoServico> AgendamentoServico { get; set; }
        public DbSet<BarbeiroHorario> BarbeiroHorario { get; set; }
        public DbSet<BarbeiroHorarioExcecao> BarbeiroHorarioExcecao { get; set; }
        public DbSet<CategoriaServico> CategoriaServico { get; set; }
        public DbSet<BarbeiroServico> BarbeiroServico { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<CategoriaServico>(entity =>
            {
                // Define o nome da tabela como 'categoriaservico' (tudo em minúsculo)
                entity.ToTable("categoriaservico");

                // Define a chave primária
                entity.HasKey(e => e.Id);

                // Mapeia a propriedade 'Id' para a coluna 'id'
                entity.Property(e => e.Id).HasColumnName("id");

                // Mapeia a propriedade 'Descricao' para a coluna 'descricao'
                // e a define como obrigatória (NOT NULL no banco)
                // Renomeei para 'Nome' para consistência, mas mantive 'Descricao' como você pediu.
                entity.Property(e => e.Descricao)
                      .HasColumnName("descricao")
                      .IsRequired();


                entity.Property(e => e.DtInicio).HasColumnName("dtinicio");
                entity.Property(e => e.DtFim).HasColumnName("dtfim");

                // Configura o relacionamento "Um-para-Muitos"
                // Uma CategoriaServico TEM MUITOS Servicos...
                // ...e cada Servico TEM UMA CategoriaServico.
                // A chave estrangeira ('IdCategoriaServico') está definida na entidade 'Servico'.
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
                entity.Property(e => e.Acesso).HasColumnName("acesso")
                .HasConversion<string>();
                entity.Property(e => e.Descricao).HasColumnName("descricao");
                entity.Property(e => e.DtCadastro).HasColumnName("dtcadastro");
                entity.Property(e => e.DtDemissao).HasColumnName("dtdemissao");
            });

            // Api.Infraestrutura.Contexto/Contexto.cs
            modelBuilder.Entity<Servico>(entity =>
            {
                entity.ToTable("servico");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");

                // Mapeia as novas propriedades
                entity.Property(e => e.Descricao).HasColumnName("nome").IsRequired(); // Agora é 'Nome'
                entity.Property(e => e.Valor).HasColumnName("valor").HasColumnType("decimal(18,2)"); // Boa prática especificar precisão
                entity.Property(e => e.TempoEstimado).HasColumnName("tempoestimado").HasColumnType("time");
                entity.Property(e => e.DtInicio).HasColumnName("dtinicio");
                entity.Property(e => e.DtFim).HasColumnName("dtfim");

                // Mapeia o relacionamento com CategoriaServico
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
                // ... mapeie as outras colunas ...
                entity.Property(e => e.DtInicio).HasColumnName("dtinicio");
                entity.Property(e => e.DtFim).HasColumnName("dtfim");

                // --- A MÁGICA ACONTECE AQUI ---

                // 1. Mapeie a coluna da chave estrangeira
                entity.Property(e => e.IdBarbeiro).HasColumnName("idbarbeiro");

                // 2. Defina o relacionamento:
                //    - Um BarbeiroServico TEM UM Barbeiro...
                //    - ...e um Barbeiro TEM MUITOS BarbeiroServicos...
                //    - ...e a chave estrangeira em BarbeiroServico é a propriedade IdBarbeiro.
                entity.HasOne(bs => bs.Barbeiro)
                      .WithMany(b => b.BarbeiroServicos) // 'BarbeiroServicos' é a List<> na entidade Barbeiro
                      .HasForeignKey(bs => bs.IdBarbeiro);

                // Faça o mesmo para o Serviço
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
                    entity.Property(e => e.TipoDia).HasColumnName("tipodia")
                    .HasConversion<string>();
                    entity.Property(e => e.DtInicio).HasColumnName("dtinicio");
                    entity.Property(e => e.DtFim).HasColumnName("dtfim");

                    // 🟢 Ajustar o relacionamento para usar a FK explícita
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

                // Mapeamento explícito da propriedade para a coluna
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
                entity.HasOne(e => e.Servico) // ✅ CORRETO: Aponta para a entidade Servico
                .WithMany(s => s.AgendamentoServicos) // Navegação inversa em Servico
                .HasForeignKey(e => e.IdServico);
            });

        }
    }
}