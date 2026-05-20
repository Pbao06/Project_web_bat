using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Getdata1.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDecimalAndOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "_Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "_Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "_Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "_Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "_Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "_Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "_Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "_Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrderNotes",
                table: "_Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "_Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "_Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PromoCode",
                table: "_Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingFee",
                table: "_Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ShippingMethod",
                table: "_Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "_Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "_Orders");

            migrationBuilder.DropColumn(
                name: "Address2",
                table: "_Orders");

            migrationBuilder.DropColumn(
                name: "City",
                table: "_Orders");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "_Orders");

            migrationBuilder.DropColumn(
                name: "District",
                table: "_Orders");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "_Orders");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "_Orders");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "_Orders");

            migrationBuilder.DropColumn(
                name: "OrderNotes",
                table: "_Orders");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "_Orders");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "_Orders");

            migrationBuilder.DropColumn(
                name: "PromoCode",
                table: "_Orders");

            migrationBuilder.DropColumn(
                name: "ShippingFee",
                table: "_Orders");

            migrationBuilder.DropColumn(
                name: "ShippingMethod",
                table: "_Orders");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "_Orders");
        }
    }
}
