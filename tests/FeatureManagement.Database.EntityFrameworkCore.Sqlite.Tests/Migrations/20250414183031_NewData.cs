using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Migrations
{
    /// <inheritdoc />
    public partial class NewData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FeatureSettings",
                columns: new[] { "Id", "CustomFilterTypeName", "FeatureId", "FilterType", "Parameters" },
                values: new object[] { new Guid("8190cc07-7499-46f0-93fc-fe45bf828df3"), null, new Guid("d3c82992-2f12-4008-9376-da37695a2747"), 2, "{\"Start\": \"Mon, 01 May 2023 13:59:59 GMT\", \"End\": \"Sat, 01 July 2023 00:00:00 GMT\"}" });

            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "Name", "RequirementType" },
                values: new object[,]
                {
                    { new Guid("0ece94e6-75a0-4257-9e8e-180a297fa7d8"), "FeatureToUpdate", 0 },
                    { new Guid("dd323f34-2ed5-49c1-acb7-d3b4a99bad4b"), "FeatureToDelete", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FeatureSettings",
                keyColumn: "Id",
                keyValue: new Guid("8190cc07-7499-46f0-93fc-fe45bf828df3"));

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: new Guid("0ece94e6-75a0-4257-9e8e-180a297fa7d8"));

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: new Guid("dd323f34-2ed5-49c1-acb7-d3b4a99bad4b"));
        }
    }
}
