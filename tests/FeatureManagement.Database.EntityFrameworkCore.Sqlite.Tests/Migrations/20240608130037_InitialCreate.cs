// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    RequirementType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeatureSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomFilterTypeName = table.Column<string>(type: "TEXT", nullable: true),
                    FilterType = table.Column<int>(type: "INTEGER", nullable: false),
                    Parameters = table.Column<string>(type: "TEXT", nullable: false),
                    FeatureId = table.Column<Guid>(type: "TEXT", nullable: false)
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
                values: new object[,]
                {
                    { new Guid("7c81e846-dc77-4aff-bf03-8dd8bb2d3194"), "FirstFeature", 1 },
                    { new Guid("d3c82992-2f12-4008-9376-da37695a2747"), "SecondFeature", 1 }
                });

            migrationBuilder.InsertData(
                table: "FeatureSettings",
                columns: new[] { "Id", "CustomFilterTypeName", "FeatureId", "FilterType", "Parameters" },
                values: new object[] { new Guid("672dc1bd-9c5b-44ce-8461-234b262a8395"), null, new Guid("7c81e846-dc77-4aff-bf03-8dd8bb2d3194"), 2, "{\"Start\": \"Mon, 01 May 2023 13:59:59 GMT\", \"End\": \"Sat, 01 July 2023 00:00:00 GMT\"}" });

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
