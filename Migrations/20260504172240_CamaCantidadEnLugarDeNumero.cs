using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gestor_Inventario_H.Migrations
{
    /// <inheritdoc />
    public partial class CamaCantidadEnLugarDeNumero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Camas");

            migrationBuilder.AddColumn<int>(
                name: "Cantidad",
                table: "Camas",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cantidad",
                table: "Camas");

            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "Camas",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
