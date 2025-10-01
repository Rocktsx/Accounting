using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations
{
    /// <inheritdoc />
    public partial class add_foreign_key_behavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppSubjectCategories_AppAccountTypes_AccountTypeId",
                table: "AppSubjectCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_AppSubjects_AppAccountTypes_AccountTypeId",
                table: "AppSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_AppSubjects_AppSubjectCategories_SubjectCategoryId",
                table: "AppSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_AppVoucherDetails_AppCompanies_SubSubjectCode",
                table: "AppVoucherDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_AppVoucherDetails_AppSubjects_SubjectId",
                table: "AppVoucherDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_AppSubjectCategories_AppAccountTypes_AccountTypeId",
                table: "AppSubjectCategories",
                column: "AccountTypeId",
                principalTable: "AppAccountTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppSubjects_AppAccountTypes_AccountTypeId",
                table: "AppSubjects",
                column: "AccountTypeId",
                principalTable: "AppAccountTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppSubjects_AppSubjectCategories_SubjectCategoryId",
                table: "AppSubjects",
                column: "SubjectCategoryId",
                principalTable: "AppSubjectCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppVoucherDetails_AppCompanies_SubSubjectCode",
                table: "AppVoucherDetails",
                column: "SubSubjectCode",
                principalTable: "AppCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppVoucherDetails_AppSubjects_SubjectId",
                table: "AppVoucherDetails",
                column: "SubjectId",
                principalTable: "AppSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppSubjectCategories_AppAccountTypes_AccountTypeId",
                table: "AppSubjectCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_AppSubjects_AppAccountTypes_AccountTypeId",
                table: "AppSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_AppSubjects_AppSubjectCategories_SubjectCategoryId",
                table: "AppSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_AppVoucherDetails_AppCompanies_SubSubjectCode",
                table: "AppVoucherDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_AppVoucherDetails_AppSubjects_SubjectId",
                table: "AppVoucherDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_AppSubjectCategories_AppAccountTypes_AccountTypeId",
                table: "AppSubjectCategories",
                column: "AccountTypeId",
                principalTable: "AppAccountTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppSubjects_AppAccountTypes_AccountTypeId",
                table: "AppSubjects",
                column: "AccountTypeId",
                principalTable: "AppAccountTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppSubjects_AppSubjectCategories_SubjectCategoryId",
                table: "AppSubjects",
                column: "SubjectCategoryId",
                principalTable: "AppSubjectCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppVoucherDetails_AppCompanies_SubSubjectCode",
                table: "AppVoucherDetails",
                column: "SubSubjectCode",
                principalTable: "AppCompanies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppVoucherDetails_AppSubjects_SubjectId",
                table: "AppVoucherDetails",
                column: "SubjectId",
                principalTable: "AppSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
