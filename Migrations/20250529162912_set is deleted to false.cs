using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternPulse4.Migrations
{
    /// <inheritdoc />
    public partial class setisdeletedtofalse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "Password" },
                values: new object[] { new DateTime(2025, 5, 29, 16, 29, 11, 561, DateTimeKind.Utc).AddTicks(7481), "$2a$11$5dKnbgA98XHU6tqXSuTN..Rzb6APSUl9c8LnDQsUBQyZXtRpOUDX6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "Password" },
                values: new object[] { new DateTime(2025, 5, 28, 7, 5, 6, 101, DateTimeKind.Utc).AddTicks(3315), "$2a$11$9DfQoAF6vwk4kRm./jYtgOSPL102YhcGpXBV06QSqNPdFEkbkzyyO" });
        }
    }
}
