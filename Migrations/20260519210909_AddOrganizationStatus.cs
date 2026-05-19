using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMAS.API.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "organizations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "organizations");
        }
    }
}
