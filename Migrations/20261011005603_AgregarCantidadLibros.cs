using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegistroLibro.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCantidadLibros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CantidadDisponible",
                table: "Libros",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CantidadPrestamos",
                table: "Libros",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CantidadDisponible",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "CantidadPrestamos",
                table: "Libros");
        }
    }
}
