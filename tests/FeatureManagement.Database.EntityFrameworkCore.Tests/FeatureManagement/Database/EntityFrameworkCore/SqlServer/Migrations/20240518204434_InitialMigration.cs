﻿// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeatureManagement.Database.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequirementType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeatureSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomFilterTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilterType = table.Column<int>(type: "int", nullable: false),
                    Parameters = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FeatureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeatureSettings_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "Name", "RequirementType" },
                values: new object[] { new Guid("cd323051-2bd5-4b66-810d-72a41cdde93e"), "FirstFeature", 1 });

            migrationBuilder.InsertData(
                table: "FeatureSettings",
                columns: new[] { "Id", "CustomFilterTypeName", "FeatureId", "FilterType", "Parameters" },
                values: new object[] { new Guid("ae963db5-18ff-4a0f-b599-a63d5c551c50"), null, new Guid("cd323051-2bd5-4b66-810d-72a41cdde93e"), 2, "{\"Start\": \"Mon, 01 May 2023 13:59:59 GMT\", \"End\": \"Sat, 01 July 2023 00:00:00 GMT\"}" });

            migrationBuilder.CreateIndex(
                name: "IX_Features_Name",
                table: "Features",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeatureSettings_FeatureId",
                table: "FeatureSettings",
                column: "FeatureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeatureSettings");

            migrationBuilder.DropTable(
                name: "Features");
        }
    }
}