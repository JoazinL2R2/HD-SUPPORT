﻿// <auto-generated />
using System;
using HD_SUPPORT.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HD_SUPPORT.Migrations
{
    [DbContext(typeof(BancoContexto))]
    partial class BancoContextoModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("HD_SUPPORT.Models.CadastroEquip", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Disponivel")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DtEmeprestimoFinal")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DtEmeprestimoInicio")
                        .HasColumnType("TEXT");

                    b.Property<string>("HeadSet")
                        .HasColumnType("TEXT");

                    b.Property<int>("IdPatrimonio")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Modelo")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Processador")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SistemaOperacionar")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("profissional_HD")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("CadastroEquipamentos", (string)null);
                });

            modelBuilder.Entity("HD_SUPPORT.Models.CadastroHelpDesk", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Foto")
                        .HasColumnType("BLOB");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Senha")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("CadastroHD", (string)null);
                });

            modelBuilder.Entity("HD_SUPPORT.Models.CadastroUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Categoria")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("Nome")
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .HasColumnType("TEXT");

                    b.Property<string>("Telefone")
                        .HasColumnType("TEXT");

                    b.Property<string>("Telegram")
                        .HasColumnType("TEXT");

                    b.Property<string>("profissional_HD")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("CadastroUser", (string)null);
                });

            modelBuilder.Entity("HD_SUPPORT.Models.EmprestimoViewModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("EquipamentoId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("FuncionarioId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("profissional_HD")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EquipamentoId");

                    b.HasIndex("FuncionarioId");

                    b.ToTable("CadastroEmprestimos", (string)null);
                });

            modelBuilder.Entity("HD_SUPPORT.Models.EmprestimoViewModel", b =>
                {
                    b.HasOne("HD_SUPPORT.Models.CadastroEquip", "Equipamento")
                        .WithMany()
                        .HasForeignKey("EquipamentoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HD_SUPPORT.Models.CadastroUser", "Funcionario")
                        .WithMany()
                        .HasForeignKey("FuncionarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Equipamento");

                    b.Navigation("Funcionario");
                });
#pragma warning restore 612, 618
        }
    }
}
