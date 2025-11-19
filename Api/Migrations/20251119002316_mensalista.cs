using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class mensalista : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mensalista_barbeiro_IdBarbeiro",
                table: "mensalista");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "mensalista",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "IdBarbeiro",
                table: "mensalista",
                newName: "idbarbeiro");

            migrationBuilder.RenameIndex(
                name: "IX_mensalista_IdBarbeiro",
                table: "mensalista",
                newName: "IX_mensalista_idbarbeiro");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "mensalista",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_mensalista_barbeiro_idbarbeiro",
                table: "mensalista",
                column: "idbarbeiro",
                principalTable: "barbeiro",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mensalista_barbeiro_idbarbeiro",
                table: "mensalista");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "mensalista",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "idbarbeiro",
                table: "mensalista",
                newName: "IdBarbeiro");

            migrationBuilder.RenameIndex(
                name: "IX_mensalista_idbarbeiro",
                table: "mensalista",
                newName: "IX_mensalista_IdBarbeiro");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "mensalista",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_mensalista_barbeiro_IdBarbeiro",
                table: "mensalista",
                column: "IdBarbeiro",
                principalTable: "barbeiro",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
