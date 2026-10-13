using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeuralForge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addIsSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_system",
                table: "projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_system",
                table: "fragment_folders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_system",
                table: "chat_folders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_system",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "is_system",
                table: "fragment_folders");

            migrationBuilder.DropColumn(
                name: "is_system",
                table: "chat_folders");
        }
    }
}
