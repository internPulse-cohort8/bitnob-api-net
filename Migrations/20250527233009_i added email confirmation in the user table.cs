using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternPulse4.Migrations
{
    /// <inheritdoc />
    public partial class iaddedemailconfirmationintheusertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailConfirmationToken",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailConfirmed",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenExpiry",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "EmailConfirmationToken", "IsEmailConfirmed", "Password", "TokenExpiry" },
                values: new object[] { new DateTime(2025, 5, 27, 23, 30, 8, 716, DateTimeKind.Utc).AddTicks(7997), "dummy-token", false, "$2a$11$JQFpPFzYJmUY6A9bh/CUfOd9BedofRewUqYe808ktwsSI5Gy7g.eS", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmationToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsEmailConfirmed",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TokenExpiry",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "Password" },
                values: new object[] { new DateTime(2025, 5, 27, 10, 49, 26, 123, DateTimeKind.Utc).AddTicks(1271), "$2a$11$7mAWMsrUZ09NOQguZiBUEOHjacRLX9a/8b6YoJOgRKCEsndK8kZKa" });
        }
    }
}
