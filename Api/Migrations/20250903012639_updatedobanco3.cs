using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class updatedobanco3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_servico_barbeiro_BarbeiroId",
                table: "servico");

            migrationBuilder.DropIndex(
                name: "IX_servico_BarbeiroId",
                table: "servico");

            migrationBuilder.DropColumn(
                name: "BarbeiroId",
                table: "servico");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BarbeiroId",
                table: "servico",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_servico_BarbeiroId",
                table: "servico",
                column: "BarbeiroId");

            migrationBuilder.AddForeignKey(
                name: "FK_servico_barbeiro_BarbeiroId",
                table: "servico",
                column: "BarbeiroId",
                principalTable: "barbeiro",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
