using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternPulse4.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EmailConfirmationToken",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "Password" },
                values: new object[] { new DateTime(2025, 5, 28, 7, 5, 6, 101, DateTimeKind.Utc).AddTicks(3315), "$2a$11$9DfQoAF6vwk4kRm./jYtgOSPL102YhcGpXBV06QSqNPdFEkbkzyyO" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EmailConfirmationToken",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "Password" },
                values: new object[] { new DateTime(2025, 5, 27, 23, 30, 8, 716, DateTimeKind.Utc).AddTicks(7997), "$2a$11$JQFpPFzYJmUY6A9bh/CUfOd9BedofRewUqYe808ktwsSI5Gy7g.eS" });
        }
    }
}
