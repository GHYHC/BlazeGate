using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazeGate.Model.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class init4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActiveHealthState",
                table: "Destination",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActiveHealthStateUpdateTime",
                table: "Destination",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PassiveHealthState",
                table: "Destination",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PassiveHealthStateUpdateTime",
                table: "Destination",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveHealthState",
                table: "Destination");

            migrationBuilder.DropColumn(
                name: "ActiveHealthStateUpdateTime",
                table: "Destination");

            migrationBuilder.DropColumn(
                name: "PassiveHealthState",
                table: "Destination");

            migrationBuilder.DropColumn(
                name: "PassiveHealthStateUpdateTime",
                table: "Destination");
        }
    }
}
