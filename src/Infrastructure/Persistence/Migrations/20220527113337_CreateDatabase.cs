using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    public partial class CreateDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Expiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "bytea", nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    EmailConfirmationCode = table.Column<string>(type: "text", nullable: true),
                    EmailConfirmedCode = table.Column<string>(type: "text", nullable: true),
                    ResetPasswordCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("43db034a-98cc-42ee-8fff-c57016484f4d"), "Admin" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "EmailConfirmationCode", "EmailConfirmed", "EmailConfirmedCode", "FirstName", "LastName", "PasswordHash", "PasswordSalt", "ResetPasswordCode", "UserName" },
                values: new object[] { new Guid("6e5d8fa8-fa96-419f-9c07-3e05b96b087e"), "defaultadmin@gmail.com", null, true, null, "Default", "Admin", new byte[] { 30, 26, 213, 8, 103, 48, 182, 118, 120, 144, 112, 216, 242, 40, 40, 207, 239, 96, 119, 49, 33, 171, 55, 121, 126, 118, 156, 120, 77, 215, 156, 72, 179, 105, 183, 56, 0, 241, 155, 101, 195, 120, 71, 19, 202, 24, 89, 175, 55, 34, 225, 108, 10, 165, 18, 204, 237, 134, 207, 84, 99, 154, 47, 187 }, new byte[] { 249, 241, 17, 68, 23, 112, 83, 99, 27, 23, 64, 13, 227, 155, 63, 133, 99, 58, 201, 76, 70, 85, 152, 9, 70, 66, 108, 245, 248, 18, 81, 251, 62, 224, 104, 49, 200, 148, 2, 170, 114, 145, 100, 43, 6, 123, 10, 84, 187, 121, 24, 246, 140, 224, 220, 192, 149, 138, 206, 19, 121, 210, 255, 199, 215, 59, 144, 106, 249, 124, 89, 200, 250, 38, 182, 73, 82, 22, 204, 110, 185, 148, 190, 61, 240, 51, 253, 98, 3, 178, 132, 83, 200, 211, 185, 10, 10, 240, 232, 227, 222, 171, 218, 46, 2, 185, 11, 241, 191, 195, 67, 159, 124, 120, 65, 168, 254, 123, 30, 84, 153, 105, 64, 114, 183, 158, 125, 252 }, null, "admin" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("43db034a-98cc-42ee-8fff-c57016484f4d"), new Guid("6e5d8fa8-fa96-419f-9c07-3e05b96b087e") });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
