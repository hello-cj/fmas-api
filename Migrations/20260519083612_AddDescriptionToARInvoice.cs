using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMAS.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToARInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ar_invoices",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ar_invoices");
        }
    }
}
