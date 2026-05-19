using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMAS.API.Migrations
{
    /// <inheritdoc />
    public partial class MakeOrganizationNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "organization_id",
                table: "users",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "VendorId",
                table: "APPayments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "APPaymentAllocation",
                columns: table => new
                {
                    APPaymentAllocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    APPaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    APInvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APPaymentAllocation", x => x.APPaymentAllocationId);
                    table.ForeignKey(
                        name: "FK_APPaymentAllocation_APPayments_APPaymentId",
                        column: x => x.APPaymentId,
                        principalTable: "APPayments",
                        principalColumn: "APPaymentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ARPaymentAllocation",
                columns: table => new
                {
                    ARPaymentAllocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ARPaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ARInvoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARPaymentAllocation", x => x.ARPaymentAllocationId);
                    table.ForeignKey(
                        name: "FK_ARPaymentAllocation_ARPayments_ARPaymentId",
                        column: x => x.ARPaymentId,
                        principalTable: "ARPayments",
                        principalColumn: "ARPaymentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ARPaymentAllocation_ar_invoices_ARInvoiceId",
                        column: x => x.ARInvoiceId,
                        principalTable: "ar_invoices",
                        principalColumn: "ARInvoiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_APPayments_VendorId",
                table: "APPayments",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_ap_invoices_VendorId",
                table: "ap_invoices",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_APPaymentAllocation_APPaymentId",
                table: "APPaymentAllocation",
                column: "APPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_ARPaymentAllocation_ARInvoiceId",
                table: "ARPaymentAllocation",
                column: "ARInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ARPaymentAllocation_ARPaymentId",
                table: "ARPaymentAllocation",
                column: "ARPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ap_invoices_Vendors_VendorId",
                table: "ap_invoices",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "VendorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_APPayments_Vendors_VendorId",
                table: "APPayments",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "VendorId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ap_invoices_Vendors_VendorId",
                table: "ap_invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_APPayments_Vendors_VendorId",
                table: "APPayments");

            migrationBuilder.DropTable(
                name: "APPaymentAllocation");

            migrationBuilder.DropTable(
                name: "ARPaymentAllocation");

            migrationBuilder.DropIndex(
                name: "IX_APPayments_VendorId",
                table: "APPayments");

            migrationBuilder.DropIndex(
                name: "IX_ap_invoices_VendorId",
                table: "ap_invoices");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "APPayments");

            migrationBuilder.AlterColumn<Guid>(
                name: "organization_id",
                table: "users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
