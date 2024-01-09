﻿// <auto-generated />
using System;
using CarRentalHub.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CarRentalHub.Migrations
{
    [DbContext(typeof(CarRentalHubContext))]
    [Migration("20240108195852_Delete table")]
    partial class Deletetable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CarRentalHub.Models.Car", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("BodyType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CarModel")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FuelType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Generation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Mileage")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("SelectedFileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SelectedPhotoId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VehicleBrand")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("YearOfProduction")
                        .HasColumnType("int");

                    b.Property<string>("YearsList")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("CarInfoModel");
                });
#pragma warning restore 612, 618
        }
    }
}
