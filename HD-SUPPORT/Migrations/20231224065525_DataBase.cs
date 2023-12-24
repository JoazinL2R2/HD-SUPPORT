using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HD_SUPPORT.Migrations
{
    /// <inheritdoc />
    public partial class DataBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DtEmeprestimoFinal",
                table: "CadastroEmprestimos");

            migrationBuilder.DropColumn(
                name: "DtEmeprestimoInicio",
                table: "CadastroEmprestimos");

            migrationBuilder.DropColumn(
                name: "HeadSet",
                table: "CadastroEmprestimos");

            migrationBuilder.DropColumn(
                name: "Modelo",
                table: "CadastroEmprestimos");

            migrationBuilder.DropColumn(
                name: "Processador",
                table: "CadastroEmprestimos");

            migrationBuilder.DropColumn(
                name: "SistemaOperacionar",
                table: "CadastroEmprestimos");

            migrationBuilder.RenameColumn(
                name: "IdPatrimonio",
                table: "CadastroEmprestimos",
                newName: "FuncionarioId");

            migrationBuilder.RenameColumn(
                name: "Disponivel",
                table: "CadastroEmprestimos",
                newName: "EquipamentoId");

            migrationBuilder.CreateTable(
                name: "CadastroEquipamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdPatrimonio = table.Column<int>(type: "INTEGER", nullable: false),
                    Modelo = table.Column<string>(type: "TEXT", nullable: true),
                    Processador = table.Column<string>(type: "TEXT", nullable: true),
                    SistemaOperacionar = table.Column<string>(type: "TEXT", nullable: true),
                    HeadSet = table.Column<string>(type: "TEXT", nullable: true),
                    DtEmeprestimoInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DtEmeprestimoFinal = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Disponivel = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CadastroEquipamentos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CadastroEmprestimos_EquipamentoId",
                table: "CadastroEmprestimos",
                column: "EquipamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_CadastroEmprestimos_FuncionarioId",
                table: "CadastroEmprestimos",
                column: "FuncionarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_CadastroEmprestimos_CadastroEquipamentos_EquipamentoId",
                table: "CadastroEmprestimos",
                column: "EquipamentoId",
                principalTable: "CadastroEquipamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CadastroEmprestimos_CadastroUser_FuncionarioId",
                table: "CadastroEmprestimos",
                column: "FuncionarioId",
                principalTable: "CadastroUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CadastroEmprestimos_CadastroEquipamentos_EquipamentoId",
                table: "CadastroEmprestimos");

            migrationBuilder.DropForeignKey(
                name: "FK_CadastroEmprestimos_CadastroUser_FuncionarioId",
                table: "CadastroEmprestimos");

            migrationBuilder.DropTable(
                name: "CadastroEquipamentos");

            migrationBuilder.DropIndex(
                name: "IX_CadastroEmprestimos_EquipamentoId",
                table: "CadastroEmprestimos");

            migrationBuilder.DropIndex(
                name: "IX_CadastroEmprestimos_FuncionarioId",
                table: "CadastroEmprestimos");

            migrationBuilder.RenameColumn(
                name: "FuncionarioId",
                table: "CadastroEmprestimos",
                newName: "IdPatrimonio");

            migrationBuilder.RenameColumn(
                name: "EquipamentoId",
                table: "CadastroEmprestimos",
                newName: "Disponivel");

            migrationBuilder.AddColumn<DateTime>(
                name: "DtEmeprestimoFinal",
                table: "CadastroEmprestimos",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DtEmeprestimoInicio",
                table: "CadastroEmprestimos",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "HeadSet",
                table: "CadastroEmprestimos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Modelo",
                table: "CadastroEmprestimos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Processador",
                table: "CadastroEmprestimos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SistemaOperacionar",
                table: "CadastroEmprestimos",
                type: "TEXT",
                nullable: true);
        }
    }
}
