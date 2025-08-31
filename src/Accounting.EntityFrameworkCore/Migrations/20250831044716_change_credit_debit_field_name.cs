using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations
{
    /// <inheritdoc />
    public partial class change_credit_debit_field_name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreditDebit",
                table: "AppSubjects",
                newName: "DebitorCreditor");

            migrationBuilder.RenameColumn(
                name: "CreditDebit",
                table: "AppSubjectCategories",
                newName: "DebitorCreditor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DebitorCreditor",
                table: "AppSubjects",
                newName: "CreditDebit");

            migrationBuilder.RenameColumn(
                name: "DebitorCreditor",
                table: "AppSubjectCategories",
                newName: "CreditDebit");
        }
    }
}
