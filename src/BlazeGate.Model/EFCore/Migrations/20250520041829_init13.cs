using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazeGate.Model.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class init13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthRsaKey",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false, comment: "服务ID"),
                    ServiceName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, comment: "服务名称"),
                    PublicKey = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false, comment: "RSA 公钥"),
                    PrivateKey = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false, comment: "RSA 私钥"),
                    CreateTime = table.Column<DateTime>(type: "dateTime", nullable: false, comment: "创建时间"),
                    UpdateTime = table.Column<DateTime>(type: "dateTime", nullable: false, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthRsaKey", x => x.Id);
                },
                comment: "授权RSA秘密");

            migrationBuilder.CreateIndex(
                name: "IX_AuthRsaKey_ServiceId",
                table: "AuthRsaKey",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthRsaKey_ServiceName",
                table: "AuthRsaKey",
                column: "ServiceName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthRsaKey");
        }
    }
}
