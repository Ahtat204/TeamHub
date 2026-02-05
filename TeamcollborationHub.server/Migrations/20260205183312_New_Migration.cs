using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamcollborationHub.server.Migrations
{
    /// <inheritdoc />
    public partial class New_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Projects_projectId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "projectId",
                table: "Users",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "nvarchar(20)",
                table: "Users",
                newName: "name");

            migrationBuilder.RenameIndex(
                name: "IX_Users_projectId",
                table: "Users",
                newName: "IX_Users_ProjectId");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Users",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedDate",
                table: "Task",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateTime",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDateTime",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Projects_ProjectId",
                table: "Users",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Projects_ProjectId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StartedDate",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "EndDateTime",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "LastModifiedDateTime",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Users",
                newName: "projectId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Users",
                newName: "nvarchar(20)");

            migrationBuilder.RenameIndex(
                name: "IX_Users_ProjectId",
                table: "Users",
                newName: "IX_Users_projectId");

            migrationBuilder.AlterColumn<int>(
                name: "projectId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Projects_projectId",
                table: "Users",
                column: "projectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
