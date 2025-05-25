using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazeGate.Model.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Destination",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ServiceConfigId = table.Column<long>(type: "bigint", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "dateTime", nullable: false, comment: "创建时间"),
                    UpdateTime = table.Column<DateTime>(type: "dateTime", nullable: false, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destination", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Page",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    ParentPageId = table.Column<long>(type: "bigint", nullable: false),
                    IndexNumber = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    SubPath = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "dateTime", nullable: false, comment: "创建时间"),
                    UpdateTime = table.Column<DateTime>(type: "dateTime", nullable: false, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Page", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "dateTime", nullable: false, comment: "创建时间"),
                    UpdateTime = table.Column<DateTime>(type: "dateTime", nullable: false, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePage",
                columns: table => new
                {
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    PageId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePage", x => new { x.RoleId, x.PageId, x.ServiceId });
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false, comment: "服务名称"),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "dateTime", nullable: false, comment: "创建时间"),
                    UpdateTime = table.Column<DateTime>(type: "dateTime", nullable: false, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceConfig",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LoadBalancingPolicy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    AuthorizationPolicy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    RateLimiterPolicy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ActiveHealthCheckEnabled = table.Column<bool>(type: "bit", nullable: false),
                    ActiveHealthCheckInterval = table.Column<int>(type: "int", nullable: false),
                    ActiveHealthCheckTimeout = table.Column<int>(type: "int", nullable: false),
                    ActiveHealthCheckPolicy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ActiveHealthCheckPath = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ActiveHealthCheckThreshold = table.Column<int>(type: "int", nullable: false),
                    PassiveHealthCheckEnabled = table.Column<bool>(type: "bit", nullable: false),
                    PassiveHealthCheckPolicy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PassiveHealthCheckReactivationPeriod = table.Column<int>(type: "int", nullable: false),
                    PassiveHealthCheckFailureRate = table.Column<float>(type: "real", nullable: false),
                    SessionAffinityEnabled = table.Column<bool>(type: "bit", nullable: false),
                    SessionAffinityFailurePolicy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SessionAffinityPolicy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SessionAffinityKeyName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    SessionAffinityCookieHttpOnly = table.Column<bool>(type: "bit", nullable: false),
                    SessionAffinityCookieMaxAge = table.Column<int>(type: "int", nullable: false),
                    DestinationPolicy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ConsulAddress = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ConsulDatacenter = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "dateTime", nullable: false, comment: "创建时间"),
                    UpdateTime = table.Column<DateTime>(type: "dateTime", nullable: false, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: false),
                    Account = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(28)", maxLength: 28, nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "dateTime", nullable: false, comment: "创建时间"),
                    UpdateTime = table.Column<DateTime>(type: "dateTime", nullable: false, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.RoleId, x.UserId, x.ServiceId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Destination_ServiceConfigId",
                table: "Destination",
                column: "ServiceConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Page_ParentPageId",
                table: "Page",
                column: "ParentPageId");

            migrationBuilder.CreateIndex(
                name: "IX_Page_ServiceId",
                table: "Page",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_ServiceId",
                table: "Role",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Service_ServiceName",
                table: "Service",
                column: "ServiceName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceConfig_ServiceId",
                table: "ServiceConfig",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceConfig_ServiceName",
                table: "ServiceConfig",
                column: "ServiceName");

            migrationBuilder.CreateIndex(
                name: "IX_User_Account",
                table: "User",
                column: "Account",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Phone",
                table: "User",
                column: "Phone",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Destination");

            migrationBuilder.DropTable(
                name: "Page");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "RolePage");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "ServiceConfig");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "UserRole");
        }
    }
}
