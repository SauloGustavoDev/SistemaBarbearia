using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class adicionadoCAtegoriaServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoriaServicoId",
                table: "servico",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdCategoriaServico",
                table: "servico",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CategoriaServico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaServico", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_servico_CategoriaServicoId",
                table: "servico",
                column: "CategoriaServicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_servico_CategoriaServico_CategoriaServicoId",
                table: "servico",
                column: "CategoriaServicoId",
                principalTable: "CategoriaServico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_servico_CategoriaServico_CategoriaServicoId",
                table: "servico");

            migrationBuilder.DropTable(
                name: "CategoriaServico");

            migrationBuilder.DropIndex(
                name: "IX_servico_CategoriaServicoId",
                table: "servico");

            migrationBuilder.DropColumn(
                name: "CategoriaServicoId",
                table: "servico");

            migrationBuilder.DropColumn(
                name: "IdCategoriaServico",
                table: "servico");
        }
    }
}
