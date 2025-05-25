using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazeGate.Model.Sample.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class init1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_Dictionary",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "类型"),
                    Key = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false, comment: "键"),
                    Value = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "值"),
                    Extended = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "扩展"),
                    Extended2 = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "扩展2"),
                    Extended3 = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "扩展3"),
                    Enabled = table.Column<bool>(type: "bit", nullable: false, comment: "是否启用"),
                    NumberIndex = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    CreateTime = table.Column<DateTime>(type: "dateTime", nullable: false, comment: "创建时间"),
                    UpdateTime = table.Column<DateTime>(type: "dateTime", nullable: false, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Dictionary", x => x.Id);
                },
                comment: "字典表");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Dictionary_Key",
                table: "TB_Dictionary",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Dictionary_Type",
                table: "TB_Dictionary",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Dictionary_Type_Key",
                table: "TB_Dictionary",
                columns: new[] { "Type", "Key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_Dictionary");
        }
    }
}
