﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using smart.database;

#nullable disable

namespace smart.database.Migrations
{
    [DbContext(typeof(SmartContext))]
    [Migration("20221104194402_HandlerProperties")]
    partial class HandlerProperties
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("smart.database.ElementHandler", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("Connected")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("ElementType")
                        .HasColumnType("int");

                    b.Property<bool>("Enabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("SettingsData")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("ElementHandlers");
                });

            modelBuilder.Entity("smart.database.HomeElement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ConnectionInfo")
                        .HasColumnType("longtext");

                    b.Property<int>("ElementHandlerId")
                        .HasColumnType("int");

                    b.Property<int>("ElementType")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("SettingsData")
                        .HasColumnType("longtext");

                    b.Property<string>("StateData")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("ElementHandlerId");

                    b.ToTable("Elements");
                });

            modelBuilder.Entity("smart.database.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Locked")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("smart.database.HomeElement", b =>
                {
                    b.HasOne("smart.database.ElementHandler", "ElementHandler")
                        .WithMany("HomeElements")
                        .HasForeignKey("ElementHandlerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ElementHandler");
                });

            modelBuilder.Entity("smart.database.User", b =>
                {
                    b.OwnsMany("smart.database.RefreshToken", "RefreshTokens", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            b1.Property<DateTime>("Created")
                                .HasColumnType("datetime(6)");

                            b1.Property<string>("CreatedByIp")
                                .IsRequired()
                                .HasColumnType("longtext");

                            b1.Property<DateTime>("Expires")
                                .HasColumnType("datetime(6)");

                            b1.Property<string>("ReasonRevoked")
                                .HasColumnType("longtext");

                            b1.Property<string>("ReplacedByToken")
                                .HasColumnType("longtext");

                            b1.Property<DateTime?>("Revoked")
                                .HasColumnType("datetime(6)");

                            b1.Property<string>("RevokedByIp")
                                .HasColumnType("longtext");

                            b1.Property<string>("Token")
                                .IsRequired()
                                .HasColumnType("longtext");

                            b1.Property<int>("UserId")
                                .HasColumnType("int");

                            b1.HasKey("Id");

                            b1.HasIndex("UserId");

                            b1.ToTable("RefreshToken");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.Navigation("RefreshTokens");
                });

            modelBuilder.Entity("smart.database.ElementHandler", b =>
                {
                    b.Navigation("HomeElements");
                });
#pragma warning restore 612, 618
        }
    }
}
