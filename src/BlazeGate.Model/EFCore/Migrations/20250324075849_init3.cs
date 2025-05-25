using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazeGate.Model.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsulAddress",
                table: "ServiceConfig");

            migrationBuilder.DropColumn(
                name: "ConsulDatacenter",
                table: "ServiceConfig");

            migrationBuilder.DropColumn(
                name: "DestinationPolicy",
                table: "ServiceConfig");

            migrationBuilder.RenameColumn(
                name: "ServiceConfigId",
                table: "Destination",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_Destination_ServiceConfigId",
                table: "Destination",
                newName: "IX_Destination_ServiceId");

            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "Destination",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Destination_ServiceName",
                table: "Destination",
                column: "ServiceName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Destination_ServiceName",
                table: "Destination");

            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "Destination");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Destination",
                newName: "ServiceConfigId");

            migrationBuilder.RenameIndex(
                name: "IX_Destination_ServiceId",
                table: "Destination",
                newName: "IX_Destination_ServiceConfigId");

            migrationBuilder.AddColumn<string>(
                name: "ConsulAddress",
                table: "ServiceConfig",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConsulDatacenter",
                table: "ServiceConfig",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DestinationPolicy",
                table: "ServiceConfig",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }
    }
}
