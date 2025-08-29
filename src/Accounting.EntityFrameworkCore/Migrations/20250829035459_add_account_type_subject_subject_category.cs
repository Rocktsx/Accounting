using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations
{
    /// <inheritdoc />
    public partial class add_account_type_subject_subject_category : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppAccountTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 30, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    OtherName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TrialBalanceSort = table.Column<int>(type: "int", nullable: false),
                    ProfitAndLossSort = table.Column<int>(type: "int", nullable: false),
                    BalanceSheetSort = table.Column<int>(type: "int", nullable: false),
                    TrialBalanceGroup = table.Column<int>(type: "int", nullable: false),
                    ProfitAndLossGroup = table.Column<int>(type: "int", nullable: false),
                    BalanceSheetGroup = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppAccountTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppSubjectCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    OtherName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreditDebit = table.Column<int>(type: "int", nullable: false),
                    AccountTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShowDetail = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSubjectCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppSubjectCategories_AppAccountTypes_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "AppAccountTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppSubjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    OtherName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SubjectCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AccountTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreditDebit = table.Column<int>(type: "int", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsSubSujectType = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsPayMethod = table.Column<bool>(type: "bit", nullable: false),
                    SeqCode = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppSubjects_AppAccountTypes_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "AppAccountTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppSubjects_AppSubjectCategories_SubjectCategoryId",
                        column: x => x.SubjectCategoryId,
                        principalTable: "AppSubjectCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppAccountTypes_TenantId_Code",
                table: "AppAccountTypes",
                columns: new[] { "TenantId", "Code" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppSubjectCategories_AccountTypeId",
                table: "AppSubjectCategories",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSubjectCategories_TenantId_Code",
                table: "AppSubjectCategories",
                columns: new[] { "TenantId", "Code" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppSubjects_AccountTypeId",
                table: "AppSubjects",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSubjects_SubjectCategoryId",
                table: "AppSubjects",
                column: "SubjectCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSubjects_TenantId_Code",
                table: "AppSubjects",
                columns: new[] { "TenantId", "Code" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSubjects");

            migrationBuilder.DropTable(
                name: "AppSubjectCategories");

            migrationBuilder.DropTable(
                name: "AppAccountTypes");
        }
    }
}
