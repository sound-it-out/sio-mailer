﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SIO.Infrastructure.EntityFrameworkCore.DbContexts;

#nullable disable

namespace SIO.Migrations.Migrations.SIO.Projection
{
    [DbContext(typeof(SIOProjectionDbContext))]
    [Migration("20220113193811_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("SIO.Domain.Emails.Projections.EmailFailure", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Body")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Error")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Recipients")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Subject")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Subject");

                    b.ToTable("EmailFailure", (string)null);
                });

            modelBuilder.Entity("SIO.Domain.Emails.Projections.EmailQueue", b =>
                {
                    b.Property<string>("Subject")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Attempts")
                        .HasColumnType("int");

                    b.Property<string>("Payload")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("PublicationDate")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Subject");

                    b.ToTable("EmailQueue", (string)null);
                });

            modelBuilder.Entity("SIO.Domain.Users.Projections.User", b =>
                {
                    b.Property<string>("Subject")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Verified")
                        .HasColumnType("bit");

                    b.HasKey("Subject");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("SIO.Infrastructure.EntityFrameworkCore.Entities.ProjectionState", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset?>("LastModifiedDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<long>("Position")
                        .HasColumnType("bigint");

                    b.HasKey("Name");

                    b.ToTable("ProjectionState", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
