using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class CommentOneToOne2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0865e601-8ccc-403c-839c-3a42b24ce3ee");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f14fede6-72cd-4b82-baf3-c401ea591bcb");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6d263c90-1708-4429-8d90-fe48f7b16462", null, "Admin", "ADMIN" },
                    { "9755f7ee-8fdf-478a-8327-65c85d11fc20", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6d263c90-1708-4429-8d90-fe48f7b16462");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9755f7ee-8fdf-478a-8327-65c85d11fc20");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0865e601-8ccc-403c-839c-3a42b24ce3ee", null, "Admin", "ADMIN" },
                    { "f14fede6-72cd-4b82-baf3-c401ea591bcb", null, "User", "USER" }
                });
        }
    }
}
