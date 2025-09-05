using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations
{
    /// <inheritdoc />
    public partial class change_subject_field_name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSubSujectType",
                table: "AppSubjects",
                newName: "IsSubSubjectType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsSubSubjectType",
                table: "AppSubjects",
                newName: "IsSubSujectType");
        }
    }
}
