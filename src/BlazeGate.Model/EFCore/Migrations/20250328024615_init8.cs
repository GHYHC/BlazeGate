using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazeGate.Model.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class init8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "RolePage",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "Role",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "Page",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RolePage_ServiceName",
                table: "RolePage",
                column: "ServiceName");

            migrationBuilder.CreateIndex(
                name: "IX_Role_ServiceName",
                table: "Role",
                column: "ServiceName");

            migrationBuilder.CreateIndex(
                name: "IX_Page_ServiceName",
                table: "Page",
                column: "ServiceName");

            migrationBuilder.CreateIndex(
                name: "IX_Destination_Address",
                table: "Destination",
                column: "Address");

            migrationBuilder.CreateIndex(
                name: "IX_Destination_Address_ServiceName",
                table: "Destination",
                columns: new[] { "Address", "ServiceName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RolePage_ServiceName",
                table: "RolePage");

            migrationBuilder.DropIndex(
                name: "IX_Role_ServiceName",
                table: "Role");

            migrationBuilder.DropIndex(
                name: "IX_Page_ServiceName",
                table: "Page");

            migrationBuilder.DropIndex(
                name: "IX_Destination_Address",
                table: "Destination");

            migrationBuilder.DropIndex(
                name: "IX_Destination_Address_ServiceName",
                table: "Destination");

            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "RolePage");

            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "Page");
        }
    }
}
