using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_web_api.Migrations
{
    /// <inheritdoc />
    public partial class FullUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProfilePicturoPublic",
                table: "UserProfiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProfilePicturoPublic",
                table: "UserProfiles");
        }
    }
}
