using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FeatureManagement.Database.EntityFrameworkCore.Tests.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FeatureSettings",
                keyColumn: "Id",
                keyValue: new Guid("ae963db5-18ff-4a0f-b599-a63d5c551c50"));

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: new Guid("cd323051-2bd5-4b66-810d-72a41cdde93e"));

            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "Name", "RequirementType" },
                values: new object[,]
                {
                    { new Guid("e02ab394-42c6-412e-9f32-7f5d2ca8e7b6"), "FirstFeature", 1 },
                    { new Guid("f5665e3e-da77-45f1-806b-6e5e9329a63f"), "SecondFeature", 1 }
                });

            migrationBuilder.InsertData(
                table: "FeatureSettings",
                columns: new[] { "Id", "CustomFilterTypeName", "FeatureId", "FilterType", "Parameters" },
                values: new object[] { new Guid("301f6510-3d8b-446d-8ff6-b68b2ab04ee6"), null, new Guid("e02ab394-42c6-412e-9f32-7f5d2ca8e7b6"), 2, "{\"Start\": \"Mon, 01 May 2023 13:59:59 GMT\", \"End\": \"Sat, 01 July 2023 00:00:00 GMT\"}" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FeatureSettings",
                keyColumn: "Id",
                keyValue: new Guid("301f6510-3d8b-446d-8ff6-b68b2ab04ee6"));

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: new Guid("f5665e3e-da77-45f1-806b-6e5e9329a63f"));

            migrationBuilder.DeleteData(
                table: "Features",
                keyColumn: "Id",
                keyValue: new Guid("e02ab394-42c6-412e-9f32-7f5d2ca8e7b6"));

            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "Id", "Name", "RequirementType" },
                values: new object[] { new Guid("cd323051-2bd5-4b66-810d-72a41cdde93e"), "FirstFeature", 1 });

            migrationBuilder.InsertData(
                table: "FeatureSettings",
                columns: new[] { "Id", "CustomFilterTypeName", "FeatureId", "FilterType", "Parameters" },
                values: new object[] { new Guid("ae963db5-18ff-4a0f-b599-a63d5c551c50"), null, new Guid("cd323051-2bd5-4b66-810d-72a41cdde93e"), 2, "{\"Start\": \"Mon, 01 May 2023 13:59:59 GMT\", \"End\": \"Sat, 01 July 2023 00:00:00 GMT\"}" });
        }
    }
}