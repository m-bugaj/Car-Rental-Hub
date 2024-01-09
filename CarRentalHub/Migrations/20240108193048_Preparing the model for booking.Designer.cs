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
    [Migration("20240108193048_Preparing the model for booking")]
    partial class Preparingthemodelforbooking
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

            modelBuilder.Entity("CarRentalHub.Models.Reservation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CarID")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CarID");

                    b.ToTable("Reservation");
                });

            modelBuilder.Entity("CarRentalHub.Models.Reservation", b =>
                {
                    b.HasOne("CarRentalHub.Models.Car", null)
                        .WithMany("Reservations")
                        .HasForeignKey("CarID");
                });

            modelBuilder.Entity("CarRentalHub.Models.Car", b =>
                {
                    b.Navigation("Reservations");
                });
#pragma warning restore 612, 618
        }
    }
}
