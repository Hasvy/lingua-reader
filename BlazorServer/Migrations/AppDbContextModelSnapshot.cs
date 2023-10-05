﻿// <auto-generated />
using System;
using BlazorServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BlazorServer.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

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

                    b.ToTable("BookCovers");
                });

            modelBuilder.Entity("Objects.Entities.Books.AbstractBook", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Id");

                    b.Property<Guid>("BookCoverId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("BookCoverId");

                    b.ToTable("AbstractBooks");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("Objects.Entities.Books.EpubBook.BookSection", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("EpubBookId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("OrderNumber")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EpubBookId");

                    b.ToTable("BookSections");
                });

            modelBuilder.Entity("Objects.Entities.Books.EpubBook.EpubBook", b =>
                {
                    b.HasBaseType("Objects.Entities.Books.AbstractBook");

                    b.Property<int>("SectionsCount")
                        .HasColumnType("int");

                    b.ToTable("EpubBooks", (string)null);
                });

            modelBuilder.Entity("Objects.Entities.Books.PdfBook.PdfBook", b =>
                {
                    b.HasBaseType("Objects.Entities.Books.AbstractBook");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("PdfBooks", (string)null);
                });

            modelBuilder.Entity("Objects.Entities.Books.AbstractBook", b =>
                {
                    b.HasOne("Objects.Entities.BookCover", "BookCover")
                        .WithMany()
                        .HasForeignKey("BookCoverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BookCover");
                });

            modelBuilder.Entity("Objects.Entities.Books.EpubBook.BookSection", b =>
                {
                    b.HasOne("Objects.Entities.Books.EpubBook.EpubBook", null)
                        .WithMany("Sections")
                        .HasForeignKey("EpubBookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Objects.Entities.Books.EpubBook.EpubBook", b =>
                {
                    b.HasOne("Objects.Entities.Books.AbstractBook", null)
                        .WithOne()
                        .HasForeignKey("Objects.Entities.Books.EpubBook.EpubBook", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Objects.Entities.Books.PdfBook.PdfBook", b =>
                {
                    b.HasOne("Objects.Entities.Books.AbstractBook", null)
                        .WithOne()
                        .HasForeignKey("Objects.Entities.Books.PdfBook.PdfBook", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Objects.Entities.Books.EpubBook.EpubBook", b =>
                {
                    b.Navigation("Sections");
                });
#pragma warning restore 612, 618
        }
    }
}
