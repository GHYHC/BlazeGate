using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazeGate.Model.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthWhiteList",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "dateTime", nullable: false, comment: "创建时间"),
                    UpdateTime = table.Column<DateTime>(type: "dateTime", nullable: false, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthWhiteList", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthWhiteList_ServiceId",
                table: "AuthWhiteList",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthWhiteList_ServiceName",
                table: "AuthWhiteList",
                column: "ServiceName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthWhiteList");
        }
    }
}
