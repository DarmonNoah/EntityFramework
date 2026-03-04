using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Migrations
{
    /// <inheritdoc />
    public partial class AdvancedModeling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Patients",
                newName: "Address_Street");

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "Patients",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Country",
                table: "Patients",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_PostalCode",
                table: "Patients",
                type: "TEXT",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ParentDepartmentId",
                table: "Departments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    HireDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Salary = table.Column<decimal>(type: "TEXT", nullable: false),
                    StaffType = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    Function = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Specialty = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    LicenseNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Service = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Grade = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ParentDepartmentId",
                table: "Departments",
                column: "ParentDepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Departments_ParentDepartmentId",
                table: "Departments",
                column: "ParentDepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Departments_ParentDepartmentId",
                table: "Departments");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropIndex(
                name: "IX_Departments_ParentDepartmentId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Address_Country",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Address_PostalCode",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ParentDepartmentId",
                table: "Departments");

            migrationBuilder.RenameColumn(
                name: "Address_Street",
                table: "Patients",
                newName: "Address");
        }
    }
}
