﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HD_SUPPORT.Migrations
{
    public partial class criacaoBDSqlite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CadastroHD",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Senha = table.Column<string>(type: "TEXT", nullable: true),
                    Foto = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CadastroHD", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CadastroUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Telegram = table.Column<string>(type: "TEXT", nullable: true),
                    Telefone = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    Categoria = table.Column<string>(type: "TEXT", nullable: true),
                    Modelo = table.Column<string>(type: "TEXT", nullable: true),
                    Processador = table.Column<string>(type: "TEXT", nullable: true),
                    SistemaOperacional = table.Column<string>(type: "TEXT", nullable: true),
                    HeadSet = table.Column<string>(type: "TEXT", nullable: true),
                    DtEmprestimoInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DtEmprestimoFinal = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CadastroUser", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CadastroHD");

            migrationBuilder.DropTable(
                name: "CadastroUser");
        }
    }
}
