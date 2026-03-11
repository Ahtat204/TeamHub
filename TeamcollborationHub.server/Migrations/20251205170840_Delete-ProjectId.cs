using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeamcollborationHub.server.Migrations
{
    /// <inheritdoc />
    public partial class DeleteProjectId : Migration
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

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Users",
                newName: "projectId");

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
