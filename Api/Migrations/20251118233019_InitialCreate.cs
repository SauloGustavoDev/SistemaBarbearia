using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "barbeiro",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "text", nullable: false),
                    numero = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    foto = table.Column<byte[]>(type: "bytea", nullable: true),
                    acesso = table.Column<string>(type: "text", nullable: false),
                    senha = table.Column<string>(type: "text", nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: true),
                    tipoagenda = table.Column<string>(type: "text", nullable: false),
                    dtcadastro = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    dtdemissao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_barbeiro", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categoriaservico",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descricao = table.Column<string>(type: "text", nullable: false),
                    dtinicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dtfim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoriaservico", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tokenconfirmacao",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    numero = table.Column<string>(type: "text", nullable: false),
                    codigo = table.Column<int>(type: "integer", nullable: false),
                    dtcriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dtexpiracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    confirmado = table.Column<bool>(type: "boolean", nullable: false),
                    Reenviado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tokenconfirmacao", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "agendamento",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    idbarbeiro = table.Column<int>(type: "integer", nullable: false),
                    nomecliente = table.Column<string>(type: "text", nullable: false),
                    numerocliente = table.Column<string>(type: "text", nullable: false),
                    dtagendamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    metodopagamento = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agendamento", x => x.id);
                    table.ForeignKey(
                        name: "fk_agendamento_barbeiro",
                        column: x => x.idbarbeiro,
                        principalTable: "barbeiro",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "barbeirohorario",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hora = table.Column<TimeOnly>(type: "time", nullable: false),
                    tipodia = table.Column<string>(type: "text", nullable: false),
                    dtinicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dtfim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    idbarbeiro = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_barbeirohorario", x => x.id);
                    table.ForeignKey(
                        name: "FK_barbeirohorario_barbeiro_idbarbeiro",
                        column: x => x.idbarbeiro,
                        principalTable: "barbeiro",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mensalista",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "text", nullable: false),
                    numero = table.Column<string>(type: "text", nullable: false),
                    valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    dtinicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dtfim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IdBarbeiro = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mensalista", x => x.id);
                    table.ForeignKey(
                        name: "FK_mensalista_barbeiro_IdBarbeiro",
                        column: x => x.IdBarbeiro,
                        principalTable: "barbeiro",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "servico",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "text", nullable: false),
                    valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    tempoestimado = table.Column<TimeOnly>(type: "time", nullable: false),
                    dtinicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dtfim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    idcategoriaservico = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servico", x => x.id);
                    table.ForeignKey(
                        name: "FK_servico_categoriaservico_idcategoriaservico",
                        column: x => x.idcategoriaservico,
                        principalTable: "categoriaservico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "agendamentohorario",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    idagendamento = table.Column<int>(type: "integer", nullable: false),
                    idbarbeirohorario = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agendamentohorario", x => x.id);
                    table.ForeignKey(
                        name: "FK_agendamentohorario_agendamento_idagendamento",
                        column: x => x.idagendamento,
                        principalTable: "agendamento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_agendamentohorario_barbeirohorario_idbarbeirohorario",
                        column: x => x.idbarbeirohorario,
                        principalTable: "barbeirohorario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "barbeirohorarioexcecao",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dtexcecao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    motivoexcecao = table.Column<int>(type: "integer", nullable: false),
                    idbarbeirohorario = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_barbeirohorarioexcecao", x => x.id);
                    table.ForeignKey(
                        name: "FK_barbeirohorarioexcecao_barbeirohorario_idbarbeirohorario",
                        column: x => x.idbarbeirohorario,
                        principalTable: "barbeirohorario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mensalistadia",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MensalistaId = table.Column<int>(type: "integer", nullable: false),
                    DiaSemana = table.Column<string>(type: "text", nullable: false),
                    Horario = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mensalistadia", x => x.id);
                    table.ForeignKey(
                        name: "FK_mensalistadia_mensalista_MensalistaId",
                        column: x => x.MensalistaId,
                        principalTable: "mensalista",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "agendamentoservico",
                columns: table => new
                {
                    idagendamento = table.Column<int>(type: "integer", nullable: false),
                    idservico = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agendamentoservico", x => new { x.idagendamento, x.idservico });
                    table.ForeignKey(
                        name: "FK_agendamentoservico_agendamento_idagendamento",
                        column: x => x.idagendamento,
                        principalTable: "agendamento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_agendamentoservico_servico_idservico",
                        column: x => x.idservico,
                        principalTable: "servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "barbeiroservico",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dtinicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dtfim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    idbarbeiro = table.Column<int>(type: "integer", nullable: false),
                    idservico = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_barbeiroservico", x => x.id);
                    table.ForeignKey(
                        name: "FK_barbeiroservico_barbeiro_idbarbeiro",
                        column: x => x.idbarbeiro,
                        principalTable: "barbeiro",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_barbeiroservico_servico_idservico",
                        column: x => x.idservico,
                        principalTable: "servico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_agendamento_idbarbeiro",
                table: "agendamento",
                column: "idbarbeiro");

            migrationBuilder.CreateIndex(
                name: "IX_agendamentohorario_idagendamento",
                table: "agendamentohorario",
                column: "idagendamento");

            migrationBuilder.CreateIndex(
                name: "IX_agendamentohorario_idbarbeirohorario",
                table: "agendamentohorario",
                column: "idbarbeirohorario");

            migrationBuilder.CreateIndex(
                name: "IX_agendamentoservico_idservico",
                table: "agendamentoservico",
                column: "idservico");

            migrationBuilder.CreateIndex(
                name: "IX_barbeirohorario_idbarbeiro",
                table: "barbeirohorario",
                column: "idbarbeiro");

            migrationBuilder.CreateIndex(
                name: "IX_barbeirohorarioexcecao_idbarbeirohorario",
                table: "barbeirohorarioexcecao",
                column: "idbarbeirohorario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_barbeiroservico_idbarbeiro",
                table: "barbeiroservico",
                column: "idbarbeiro");

            migrationBuilder.CreateIndex(
                name: "IX_barbeiroservico_idservico",
                table: "barbeiroservico",
                column: "idservico");

            migrationBuilder.CreateIndex(
                name: "IX_mensalista_IdBarbeiro",
                table: "mensalista",
                column: "IdBarbeiro");

            migrationBuilder.CreateIndex(
                name: "IX_mensalistadia_MensalistaId",
                table: "mensalistadia",
                column: "MensalistaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_servico_idcategoriaservico",
                table: "servico",
                column: "idcategoriaservico");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "agendamentohorario");

            migrationBuilder.DropTable(
                name: "agendamentoservico");

            migrationBuilder.DropTable(
                name: "barbeirohorarioexcecao");

            migrationBuilder.DropTable(
                name: "barbeiroservico");

            migrationBuilder.DropTable(
                name: "mensalistadia");

            migrationBuilder.DropTable(
                name: "tokenconfirmacao");

            migrationBuilder.DropTable(
                name: "agendamento");

            migrationBuilder.DropTable(
                name: "barbeirohorario");

            migrationBuilder.DropTable(
                name: "servico");

            migrationBuilder.DropTable(
                name: "mensalista");

            migrationBuilder.DropTable(
                name: "categoriaservico");

            migrationBuilder.DropTable(
                name: "barbeiro");
        }
    }
}
