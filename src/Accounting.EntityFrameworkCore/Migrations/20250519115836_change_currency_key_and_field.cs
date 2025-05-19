using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations
{
    /// <inheritdoc />
    public partial class change_currency_key_and_field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppCurrencies",
                table: "AppCurrencies");

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "AppCurrencies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EffectiveDate",
                table: "AppCurrencies",
                type: "date",
                nullable: true,
                defaultValue: new DateOnly(1900, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValue: new DateOnly(1900, 1, 1));

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppCurrencies",
                table: "AppCurrencies",
                columns: new[] { "TenantId", "SourceCurrency", "TargetCurrency" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppCurrencies",
                table: "AppCurrencies");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EffectiveDate",
                table: "AppCurrencies",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1900, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true,
                oldDefaultValue: new DateOnly(1900, 1, 1));

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "AppCurrencies",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppCurrencies",
                table: "AppCurrencies",
                columns: new[] { "SourceCurrency", "TargetCurrency" });
        }
    }
}
