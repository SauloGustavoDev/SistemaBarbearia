using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class updatedobanco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_servico_barbeiro_BarbeiroId",
                table: "servico");

            migrationBuilder.RenameColumn(
                name: "IdCategoriaServico",
                table: "servico",
                newName: "IdBarbeiro");

            migrationBuilder.AlterColumn<int>(
                name: "BarbeiroId",
                table: "servico",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_servico_IdBarbeiro",
                table: "servico",
                column: "IdBarbeiro");

            migrationBuilder.AddForeignKey(
                name: "FK_servico_barbeiro_BarbeiroId",
                table: "servico",
                column: "BarbeiroId",
                principalTable: "barbeiro",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_servico_barbeiro_IdBarbeiro",
                table: "servico",
                column: "IdBarbeiro",
                principalTable: "barbeiro",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_servico_barbeiro_BarbeiroId",
                table: "servico");

            migrationBuilder.DropForeignKey(
                name: "FK_servico_barbeiro_IdBarbeiro",
                table: "servico");

            migrationBuilder.DropIndex(
                name: "IX_servico_IdBarbeiro",
                table: "servico");

            migrationBuilder.RenameColumn(
                name: "IdBarbeiro",
                table: "servico",
                newName: "IdCategoriaServico");

            migrationBuilder.AlterColumn<int>(
                name: "BarbeiroId",
                table: "servico",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_servico_barbeiro_BarbeiroId",
                table: "servico",
                column: "BarbeiroId",
                principalTable: "barbeiro",
                principalColumn: "id");
        }
    }
}
