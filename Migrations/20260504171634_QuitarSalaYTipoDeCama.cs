using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gestor_Inventario_H.Migrations
{
    /// <inheritdoc />
    public partial class QuitarSalaYTipoDeCama : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sala",
                table: "Camas");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Camas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sala",
                table: "Camas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Camas",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
