using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalManagement.Migrations
{
    /// <inheritdoc />
    public partial class PerformanceAndConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Consultations_DoctorId",
                table: "Consultations");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Patients",
                type: "BLOB",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_LastName_FirstName",
                table: "Patients",
                columns: new[] { "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_DoctorId_Date",
                table: "Consultations",
                columns: new[] { "DoctorId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_PatientId_Date",
                table: "Consultations",
                columns: new[] { "PatientId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patients_LastName_FirstName",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Consultations_DoctorId_Date",
                table: "Consultations");

            migrationBuilder.DropIndex(
                name: "IX_Consultations_PatientId_Date",
                table: "Consultations");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Patients");

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_DoctorId",
                table: "Consultations",
                column: "DoctorId");
        }
    }
}
