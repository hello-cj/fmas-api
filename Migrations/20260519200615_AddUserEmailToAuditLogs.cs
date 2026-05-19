using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMAS.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserEmailToAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "AuditLogs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "AuditLogs");
        }
    }
}
