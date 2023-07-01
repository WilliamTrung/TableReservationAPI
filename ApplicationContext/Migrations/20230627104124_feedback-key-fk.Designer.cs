﻿// <auto-generated />
using System;
using ApplicationContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApplicationContext.Migrations
{
    [DbContext(typeof(TableReservationContext))]
    [Migration("20230627104124_feedback-key-fk")]
    partial class feedbackkeyfk
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ApplicationCore.Entities.Feedback", b =>
                {
                    b.Property<int>("ReservationId")
                        .HasColumnType("integer");

                    b.Property<string>("Comment")
                        .HasColumnType("text");

                    b.Property<int?>("FacilityRating")
                        .HasColumnType("integer");

                    b.Property<int?>("FoodRating")
                        .HasColumnType("integer");

                    b.Property<int?>("ServiceRating")
                        .HasColumnType("integer");

                    b.Property<int?>("UtilityRating")
                        .HasColumnType("integer");

                    b.HasKey("ReservationId");

                    b.ToTable("Feedback");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Reservation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("GuestAmount")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Note")
                        .HasColumnType("text");

                    b.Property<bool>("Private")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("ReservedTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int?>("TableId")
                        .HasColumnType("integer");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("TableId");

                    b.HasIndex("UserId");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Table", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<int>("StatusId")
                        .HasColumnType("integer");

                    b.Property<int>("TypeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("TypeId");

                    b.ToTable("Tables");
                });

            modelBuilder.Entity("ApplicationCore.Entities.TableType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Private")
                        .HasColumnType("boolean");

                    b.Property<int>("Seat")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Seat", "Private")
                        .IsUnique();

                    b.ToTable("TableTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Private = true,
                            Seat = 2
                        },
                        new
                        {
                            Id = 2,
                            Private = false,
                            Seat = 2
                        },
                        new
                        {
                            Id = 3,
                            Private = true,
                            Seat = 4
                        },
                        new
                        {
                            Id = 4,
                            Private = false,
                            Seat = 4
                        });
                });

            modelBuilder.Entity("ApplicationCore.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("LockoutCount")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Feedback", b =>
                {
                    b.HasOne("ApplicationCore.Entities.Reservation", "Reservation")
                        .WithMany()
                        .HasForeignKey("ReservationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Reservation");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Reservation", b =>
                {
                    b.HasOne("ApplicationCore.Entities.Table", "Table")
                        .WithMany()
                        .HasForeignKey("TableId");

                    b.HasOne("ApplicationCore.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("Table");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ApplicationCore.Entities.Table", b =>
                {
                    b.HasOne("ApplicationCore.Entities.TableType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Type");
                });
#pragma warning restore 612, 618
        }
    }
}
