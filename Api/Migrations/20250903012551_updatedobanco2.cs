using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class updatedobanco2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_servico_barbeiro_IdBarbeiro",
                table: "servico");

            migrationBuilder.RenameColumn(
                name: "IdBarbeiro",
                table: "servico",
                newName: "idbarbeiro");

            migrationBuilder.RenameIndex(
                name: "IX_servico_IdBarbeiro",
                table: "servico",
                newName: "IX_servico_idbarbeiro");

            migrationBuilder.AddForeignKey(
                name: "FK_servico_barbeiro_idbarbeiro",
                table: "servico",
                column: "idbarbeiro",
                principalTable: "barbeiro",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_servico_barbeiro_idbarbeiro",
                table: "servico");

            migrationBuilder.RenameColumn(
                name: "idbarbeiro",
                table: "servico",
                newName: "IdBarbeiro");

            migrationBuilder.RenameIndex(
                name: "IX_servico_idbarbeiro",
                table: "servico",
                newName: "IX_servico_IdBarbeiro");

            migrationBuilder.AddForeignKey(
                name: "FK_servico_barbeiro_IdBarbeiro",
                table: "servico",
                column: "IdBarbeiro",
                principalTable: "barbeiro",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
