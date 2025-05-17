using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations
{
    /// <inheritdoc />
    public partial class add_currency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppCurrencies",
                columns: table => new
                {
                    SourceCurrency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TargetCurrency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SourceAmount = table.Column<decimal>(type: "decimal(20,7)", precision: 20, scale: 7, nullable: false),
                    TargetAmount = table.Column<decimal>(type: "decimal(20,7)", precision: 20, scale: 7, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(20,7)", precision: 20, scale: 7, nullable: false),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: false, defaultValue: new DateOnly(1900, 1, 1)),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCurrencies", x => new { x.SourceCurrency, x.TargetCurrency });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppCurrencies");
        }
    }
}
