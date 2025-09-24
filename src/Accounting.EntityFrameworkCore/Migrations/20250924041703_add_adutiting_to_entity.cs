using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations
{
    /// <inheritdoc />
    public partial class add_adutiting_to_entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppSubjects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppSubjects",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AppSubjects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "AppSubjects",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppSubjectCategories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppSubjectCategories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AppSubjectCategories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "AppSubjectCategories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppCurrencies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppCurrencies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AppCurrencies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "AppCurrencies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppAccountTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppAccountTypes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AppAccountTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "AppAccountTypes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppAccountingPeriods",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppAccountingPeriods",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AppAccountingPeriods",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "AppAccountingPeriods",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppSubjects");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppSubjects");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AppSubjects");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "AppSubjects");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppSubjectCategories");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppSubjectCategories");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AppSubjectCategories");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "AppSubjectCategories");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppCurrencies");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppCurrencies");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AppCurrencies");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "AppCurrencies");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppAccountTypes");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppAccountTypes");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AppAccountTypes");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "AppAccountTypes");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppAccountingPeriods");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppAccountingPeriods");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AppAccountingPeriods");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "AppAccountingPeriods");
        }
    }
}
