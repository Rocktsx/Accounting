using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations
{
    /// <inheritdoc />
    public partial class change_currency_key : Migration
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
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "AppCurrencies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppCurrencies",
                table: "AppCurrencies",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppCurrencies_TenantId_SourceCurrency_TargetCurrency",
                table: "AppCurrencies",
                columns: new[] { "TenantId", "SourceCurrency", "TargetCurrency" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AppCurrencies",
                table: "AppCurrencies");

            migrationBuilder.DropIndex(
                name: "IX_AppCurrencies_TenantId_SourceCurrency_TargetCurrency",
                table: "AppCurrencies");

            migrationBuilder.DropColumn(
                name: "Id",
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

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppCurrencies",
                table: "AppCurrencies",
                columns: new[] { "TenantId", "SourceCurrency", "TargetCurrency" });
        }
    }
}
