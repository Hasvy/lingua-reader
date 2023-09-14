﻿// <auto-generated />
using System;
using BlazorServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BlazorServer.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230914104024_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Objects.Entities.Book", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Books");

                    b.HasData(
                        new
                        {
                            Id = new Guid("d87ea8ac-7a38-4b4d-a229-c8930203249e"),
                            Text = "Text"
                        });
                });

            modelBuilder.Entity("Objects.Entities.BookCover", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Author")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("BookId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Format")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("BookId")
                        .IsUnique();

                    b.ToTable("BookCovers");

                    b.HasData(
                        new
                        {
                            Id = new Guid("0cf6923b-9a81-4e0e-89f1-744165b9f19c"),
                            Author = "Author",
                            BookId = new Guid("d87ea8ac-7a38-4b4d-a229-c8930203249e"),
                            Description = "Description",
                            Format = "epub",
                            Title = "Title2"
                        });
                });

            modelBuilder.Entity("Objects.Entities.BookCover", b =>
                {
                    b.HasOne("Objects.Entities.Book", "Book")
                        .WithOne("BookCover")
                        .HasForeignKey("Objects.Entities.BookCover", "BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");
                });

            modelBuilder.Entity("Objects.Entities.Book", b =>
                {
                    b.Navigation("BookCover")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
