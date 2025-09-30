using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareDev.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeededNewMedicationData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 1,
                columns: new[] { "Name", "Schedule" },
                values: new object[] { "None", "N/A" });

            migrationBuilder.UpdateData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 2,
                column: "Name",
                value: "Paracetamol");

            migrationBuilder.UpdateData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 3,
                columns: new[] { "Name", "Schedule" },
                values: new object[] { "Ibuprofen", "PRN" });

            migrationBuilder.UpdateData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 4,
                columns: new[] { "Name", "Schedule" },
                values: new object[] { "Amoxicillin", "Scheduled" });

            migrationBuilder.UpdateData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 5,
                columns: new[] { "Name", "Schedule" },
                values: new object[] { "Aspirin", "Schedules" });

            migrationBuilder.InsertData(
                table: "Medications",
                columns: new[] { "MedicationId", "EmployeeId", "Name", "PatientId", "Schedule", "UsageNotes" },
                values: new object[] { 6, null, "Metformin", null, "PRN", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 1,
                columns: new[] { "Name", "Schedule" },
                values: new object[] { "Paracetamol", "PRN" });

            migrationBuilder.UpdateData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 2,
                column: "Name",
                value: "Ibuprofen");

            migrationBuilder.UpdateData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 3,
                columns: new[] { "Name", "Schedule" },
                values: new object[] { "Amoxicillin", "Scheduled" });

            migrationBuilder.UpdateData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 4,
                columns: new[] { "Name", "Schedule" },
                values: new object[] { "Aspirin", "Schedules" });

            migrationBuilder.UpdateData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 5,
                columns: new[] { "Name", "Schedule" },
                values: new object[] { "Metformin", "PRN" });
        }
    }
}
