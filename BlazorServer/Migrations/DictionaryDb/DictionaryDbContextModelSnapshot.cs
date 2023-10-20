﻿// <auto-generated />
using BlazorServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BlazorServer.Migrations.DictionaryDb
{
    [DbContext(typeof(DictionaryDbContext))]
    partial class DictionaryDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Objects.Entities.Translator.WordTranslation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<float>("confidence")
                        .HasColumnType("real");

                    b.Property<string>("displayTarget")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("posTag")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("prefixWord")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("WordTranslations");
                });
#pragma warning restore 612, 618
        }
    }
}
