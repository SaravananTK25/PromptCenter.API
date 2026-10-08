using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromptCenter.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPromptFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Prompts",
                newName: "Type");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Prompts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Prompts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NegativePrompt",
                table: "Prompts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PromptText",
                table: "Prompts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Prompts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "NegativePrompt",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "PromptText",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Prompts");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Prompts",
                newName: "Content");
        }
    }
}
