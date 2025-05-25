using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazeGate.Model.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class init7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "Service",
                comment: "服务表");

            migrationBuilder.AlterColumn<bool>(
                name: "Enabled",
                table: "Service",
                type: "bit",
                nullable: false,
                comment: "启用",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                table: "Service",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "是否为系统服务");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "Service",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                comment: "Token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSystem",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "Service");

            migrationBuilder.AlterTable(
                name: "Service",
                oldComment: "服务表");

            migrationBuilder.AlterColumn<bool>(
                name: "Enabled",
                table: "Service",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "启用");
        }
    }
}
