using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addingquantitytoproduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("b12fbfc0-0f42-4b26-ad95-5172c2347ca3"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("d309b65e-0a42-4d3b-abfd-bc2388efd89d"));

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("4d529c06-6d2d-4c19-baec-0988e5e8c590"), null, " the visitor role for the user ", "User", "USER" },
                    { new Guid("55a3fbea-0df6-4340-b6aa-87f0f0516ef3"), null, " the admin role for the user ", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("4d529c06-6d2d-4c19-baec-0988e5e8c590"));

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("55a3fbea-0df6-4340-b6aa-87f0f0516ef3"));

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Products");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("b12fbfc0-0f42-4b26-ad95-5172c2347ca3"), null, " the admin role for the user ", "Admin", "ADMIN" },
                    { new Guid("d309b65e-0a42-4d3b-abfd-bc2388efd89d"), null, " the visitor role for the user ", "User", "USER" }
                });
        }
    }
}
