﻿// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

// <auto-generated />
using System;
using FeatureManagement.Database.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FeatureManagement.Database.EntityFrameworkCore.SqlServer.Migrations
{
    [DbContext(typeof(TestDbContext))]
    [Migration("20240518204434_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.18")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FeatureManagement.Database.Feature", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("RequirementType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Features");

                    b.HasData(
                        new
                        {
                            Id = new Guid("cd323051-2bd5-4b66-810d-72a41cdde93e"),
                            Name = "FirstFeature",
                            RequirementType = 1
                        });
                });

            modelBuilder.Entity("FeatureManagement.Database.FeatureSettings", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CustomFilterTypeName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("FeatureId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("FilterType")
                        .HasColumnType("int");

                    b.Property<string>("Parameters")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FeatureId");

                    b.ToTable("FeatureSettings");

                    b.HasData(
                        new
                        {
                            Id = new Guid("ae963db5-18ff-4a0f-b599-a63d5c551c50"),
                            FeatureId = new Guid("cd323051-2bd5-4b66-810d-72a41cdde93e"),
                            FilterType = 2,
                            Parameters = "{\"Start\": \"Mon, 01 May 2023 13:59:59 GMT\", \"End\": \"Sat, 01 July 2023 00:00:00 GMT\"}"
                        });
                });

            modelBuilder.Entity("FeatureManagement.Database.FeatureSettings", b =>
                {
                    b.HasOne("FeatureManagement.Database.Feature", "Feature")
                        .WithMany("Settings")
                        .HasForeignKey("FeatureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Feature");
                });

            modelBuilder.Entity("FeatureManagement.Database.Feature", b =>
                {
                    b.Navigation("Settings");
                });
#pragma warning restore 612, 618
        }
    }
}
