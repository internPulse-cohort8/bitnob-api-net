using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternPulse4.Migrations
{
    /// <inheritdoc />
    public partial class SetDefaultForIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "Password" },
                values: new object[] { new DateTime(2025, 5, 29, 11, 38, 24, 714, DateTimeKind.Utc).AddTicks(7994), "$2a$11$bJQtEOAnxrcSx1ub5PwYBOpv4X5WqlZrL4Np18UFi/cLOWs2mrEBi" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "Password" },
                values: new object[] { new DateTime(2025, 5, 27, 10, 49, 26, 123, DateTimeKind.Utc).AddTicks(1271), "$2a$11$7mAWMsrUZ09NOQguZiBUEOHjacRLX9a/8b6YoJOgRKCEsndK8kZKa" });
        }
    }
}
