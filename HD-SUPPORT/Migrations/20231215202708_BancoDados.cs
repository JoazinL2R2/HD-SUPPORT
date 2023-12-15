using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HD_SUPPORT.Migrations
{
    /// <inheritdoc />
    public partial class BancoDados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CadastroEmprestimos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdPatrimonio = table.Column<int>(type: "INTEGER", nullable: false),
                    Modelo = table.Column<string>(type: "TEXT", nullable: false),
                    Processador = table.Column<string>(type: "TEXT", nullable: false),
                    SistemaOperacionar = table.Column<string>(type: "TEXT", nullable: false),
                    HeadSet = table.Column<string>(type: "TEXT", nullable: false),
                    DtEmeprestimoInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DtEmeprestimoFinal = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Disponivel = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CadastroEmprestimos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CadastroEmprestimos");
        }
    }
}
