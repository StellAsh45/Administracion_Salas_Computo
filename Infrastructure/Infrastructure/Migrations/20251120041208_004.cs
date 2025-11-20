using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Computadores_Salas_SalaId",
                table: "Computadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Computadores_Usuarios_UsuarioId",
                table: "Computadores");

            migrationBuilder.DropIndex(
                name: "IX_Computadores_UsuarioId",
                table: "Computadores");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Computadores");

            migrationBuilder.AlterColumn<Guid>(
                name: "SalaId",
                table: "Computadores",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Computadores_Salas_SalaId",
                table: "Computadores",
                column: "SalaId",
                principalTable: "Salas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Computadores_Salas_SalaId",
                table: "Computadores");

            migrationBuilder.AlterColumn<Guid>(
                name: "SalaId",
                table: "Computadores",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "Computadores",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Computadores_UsuarioId",
                table: "Computadores",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Computadores_Salas_SalaId",
                table: "Computadores",
                column: "SalaId",
                principalTable: "Salas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Computadores_Usuarios_UsuarioId",
                table: "Computadores",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }
    }
}
