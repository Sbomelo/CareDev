using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareDev.Data.Migrations
{
    /// <inheritdoc />
    public partial class LinkedPatientToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChronicConditions_Patients_PatientId",
                table: "ChronicConditions");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Roles_RoleId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_ChronicConditions_PatientId",
                table: "ChronicConditions");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "ChronicConditions");

            migrationBuilder.AddColumn<int>(
                name: "AllergyId",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Patients",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChronicConditionId",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Patients",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MedicationId",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MedicationsMedicationId",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "Employees",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_AllergyId",
                table: "Patients",
                column: "AllergyId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_ApplicationUserId",
                table: "Patients",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_ChronicConditionId",
                table: "Patients",
                column: "ChronicConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_MedicationsMedicationId",
                table: "Patients",
                column: "MedicationsMedicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Roles_RoleId",
                table: "Employees",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Allergies_AllergyId",
                table: "Patients",
                column: "AllergyId",
                principalTable: "Allergies",
                principalColumn: "AllergyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_AspNetUsers_ApplicationUserId",
                table: "Patients",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_ChronicConditions_ChronicConditionId",
                table: "Patients",
                column: "ChronicConditionId",
                principalTable: "ChronicConditions",
                principalColumn: "ChronicConditionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Medications_MedicationsMedicationId",
                table: "Patients",
                column: "MedicationsMedicationId",
                principalTable: "Medications",
                principalColumn: "MedicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Roles_RoleId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Allergies_AllergyId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_AspNetUsers_ApplicationUserId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_ChronicConditions_ChronicConditionId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Medications_MedicationsMedicationId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_AllergyId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_ApplicationUserId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_ChronicConditionId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_MedicationsMedicationId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "AllergyId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ChronicConditionId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "MedicationId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "MedicationsMedicationId",
                table: "Patients");

            migrationBuilder.AlterColumn<int>(
                name: "RoleId",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "ChronicConditions",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ChronicConditions",
                keyColumn: "ChronicConditionId",
                keyValue: 1,
                column: "PatientId",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChronicConditions",
                keyColumn: "ChronicConditionId",
                keyValue: 2,
                column: "PatientId",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChronicConditions",
                keyColumn: "ChronicConditionId",
                keyValue: 3,
                column: "PatientId",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChronicConditions",
                keyColumn: "ChronicConditionId",
                keyValue: 4,
                column: "PatientId",
                value: null);

            migrationBuilder.UpdateData(
                table: "ChronicConditions",
                keyColumn: "ChronicConditionId",
                keyValue: 5,
                column: "PatientId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_ChronicConditions_PatientId",
                table: "ChronicConditions",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChronicConditions_Patients_PatientId",
                table: "ChronicConditions",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Roles_RoleId",
                table: "Employees",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
