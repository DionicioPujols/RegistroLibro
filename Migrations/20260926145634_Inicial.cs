using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegistroLibro.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tablas ya existen en la base de datos.
            // Se comenta este código para evitar el error "There is already an object named..."
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
