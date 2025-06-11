using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto_web_api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAttributesNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsProfilePicturoPublic",
                table: "UserProfiles",
                newName: "IsProfilePicturePublic");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsProfilePicturePublic",
                table: "UserProfiles",
                newName: "IsProfilePicturoPublic");
        }
    }
}
