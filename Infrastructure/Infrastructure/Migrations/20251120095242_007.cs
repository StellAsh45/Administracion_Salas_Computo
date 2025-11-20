using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _007 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Solicitudes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "SalaId",
                table: "Solicitudes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Solicitudes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Solicitudes_SalaId",
                table: "Solicitudes",
                column: "SalaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Solicitudes_Salas_SalaId",
                table: "Solicitudes",
                column: "SalaId",
                principalTable: "Salas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solicitudes_Salas_SalaId",
                table: "Solicitudes");

            migrationBuilder.DropIndex(
                name: "IX_Solicitudes_SalaId",
                table: "Solicitudes");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Solicitudes");

            migrationBuilder.DropColumn(
                name: "SalaId",
                table: "Solicitudes");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Solicitudes");
        }
    }
}
