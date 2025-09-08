using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations
{
    /// <inheritdoc />
    public partial class add_voucher_voucher_detail_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppVouchers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VoucherDate = table.Column<DateOnly>(type: "date", nullable: false),
                    VoucherType = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GenNo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppVouchers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppVoucherDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoucherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubSubjectCode = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DebitorCreditor = table.Column<int>(type: "int", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CurrencyRate = table.Column<decimal>(type: "decimal(20,7)", precision: 20, scale: 7, nullable: false),
                    ForeignAmount = table.Column<decimal>(type: "decimal(20,7)", precision: 20, scale: 7, nullable: false),
                    NativeAmount = table.Column<decimal>(type: "decimal(20,7)", precision: 20, scale: 7, nullable: false),
                    DocNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Project = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Custom1 = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Custom2 = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ItemQty = table.Column<int>(type: "int", nullable: false),
                    IsOriginal = table.Column<bool>(type: "bit", nullable: false),
                    PaymentReference = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppVoucherDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppVoucherDetails_AppCompanies_SubSubjectCode",
                        column: x => x.SubSubjectCode,
                        principalTable: "AppCompanies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppVoucherDetails_AppSubjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "AppSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppVoucherDetails_AppVouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "AppVouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppVoucherDetails_SubjectId",
                table: "AppVoucherDetails",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AppVoucherDetails_SubSubjectCode",
                table: "AppVoucherDetails",
                column: "SubSubjectCode");

            migrationBuilder.CreateIndex(
                name: "IX_AppVoucherDetails_VoucherId",
                table: "AppVoucherDetails",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_AppVouchers_TenantId_Code",
                table: "AppVouchers",
                columns: new[] { "TenantId", "Code" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppVoucherDetails");

            migrationBuilder.DropTable(
                name: "AppVouchers");
        }
    }
}
