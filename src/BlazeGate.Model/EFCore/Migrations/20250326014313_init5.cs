using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazeGate.Model.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class init5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveRemoveUnhealthyAfter",
                table: "ServiceConfig",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PassiveRemoveUnhealthyAfter",
                table: "ServiceConfig",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveRemoveUnhealthyAfter",
                table: "ServiceConfig");

            migrationBuilder.DropColumn(
                name: "PassiveRemoveUnhealthyAfter",
                table: "ServiceConfig");
        }
    }
}
