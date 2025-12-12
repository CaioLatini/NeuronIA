using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeuronIA.Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTokenGoogle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdUsuario",
                table: "GoogleCalendarTokens",
                newName: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "GoogleCalendarTokens",
                newName: "IdUsuario");
        }
    }
}
