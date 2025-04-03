using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_web_api.Migrations
{
    /// <inheritdoc />
    public partial class IsArchiveAttributeInPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Posts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Posts");
        }
    }
}
