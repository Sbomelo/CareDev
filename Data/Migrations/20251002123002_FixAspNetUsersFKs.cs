using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareDev.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixAspNetUsersFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicationAdministrations_Medications_MedicationId",
                table: "MedicationAdministrations");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicationAdministrations_Patients_PatientId",
                table: "MedicationAdministrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Medications_MedicationsMedicationId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_MedicationsMedicationId",
                table: "Patients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicationAdministrations",
                table: "MedicationAdministrations");

            migrationBuilder.DropColumn(
                name: "MedicationsMedicationId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Medications");

            migrationBuilder.RenameTable(
                name: "MedicationAdministrations",
                newName: "MedicationAdministration");

            migrationBuilder.RenameIndex(
                name: "IX_MedicationAdministrations_MedicationId",
                table: "MedicationAdministration",
                newName: "IX_MedicationAdministration_MedicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicationAdministration",
                table: "MedicationAdministration",
                columns: new[] { "PatientId", "MedicationId" });

            migrationBuilder.CreateTable(
                name: "MedicationDispensations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DispenserUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MedicationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Route = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScheduleLevel = table.Column<int>(type: "int", nullable: false),
                    TimeDispensed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicationDispensations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicationDispensations_AspNetUsers_DispenserUserId",
                        column: x => x.DispenserUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicationDispensations_AspNetUsers_PatientUserId",
                        column: x => x.PatientUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            

            migrationBuilder.CreateTable(
                name: "AdministeredMeds",
                columns: table => new
                {
                    AdministeredId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DispenseId = table.Column<int>(type: "int", nullable: false),
                    AdministeredById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TimeGiven = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AdverseReactions = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministeredMeds", x => x.AdministeredId);
                    table.ForeignKey(
                        name: "FK_AdministeredMeds_AspNetUsers_AdministeredById",
                        column: x => x.AdministeredById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdministeredMeds_MedicationDispensations_DispenseId",
                        column: x => x.DispenseId,
                        principalTable: "MedicationDispensations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            

            migrationBuilder.CreateIndex(
                name: "IX_Patients_MedicationId",
                table: "Patients",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_AdministeredMeds_AdministeredById",
                table: "AdministeredMeds",
                column: "AdministeredById");

            migrationBuilder.CreateIndex(
                name: "IX_AdministeredMeds_DispenseId",
                table: "AdministeredMeds",
                column: "DispenseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicationDispensations_DispenserUserId",
                table: "MedicationDispensations",
                column: "DispenserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicationDispensations_PatientUserId",
                table: "MedicationDispensations",
                column: "PatientUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationAdministration_Medications_MedicationId",
                table: "MedicationAdministration",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "MedicationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationAdministration_Patients_PatientId",
                table: "MedicationAdministration",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Medications_MedicationId",
                table: "Patients",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "MedicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicationAdministration_Medications_MedicationId",
                table: "MedicationAdministration");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicationAdministration_Patients_PatientId",
                table: "MedicationAdministration");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Medications_MedicationId",
                table: "Patients");

            migrationBuilder.DropTable(
                name: "AdministeredMeds");

            migrationBuilder.DropTable(
                name: "PatientVitals");

            migrationBuilder.DropTable(
                name: "MedicationDispensations");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_Patients_MedicationId",
                table: "Patients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicationAdministration",
                table: "MedicationAdministration");

            migrationBuilder.RenameTable(
                name: "MedicationAdministration",
                newName: "MedicationAdministrations");

            migrationBuilder.RenameIndex(
                name: "IX_MedicationAdministration_MedicationId",
                table: "MedicationAdministrations",
                newName: "IX_MedicationAdministrations_MedicationId");

            migrationBuilder.AddColumn<int>(
                name: "MedicationsMedicationId",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "Medications",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicationAdministrations",
                table: "MedicationAdministrations",
                columns: new[] { "PatientId", "MedicationId" });

            migrationBuilder.UpdateData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 1,
                column: "PatientId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 2,
                column: "PatientId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 3,
                column: "PatientId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 4,
                column: "PatientId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 5,
                column: "PatientId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Medications",
                keyColumn: "MedicationId",
                keyValue: 6,
                column: "PatientId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_MedicationsMedicationId",
                table: "Patients",
                column: "MedicationsMedicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationAdministrations_Medications_MedicationId",
                table: "MedicationAdministrations",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "MedicationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicationAdministrations_Patients_PatientId",
                table: "MedicationAdministrations",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Medications_MedicationsMedicationId",
                table: "Patients",
                column: "MedicationsMedicationId",
                principalTable: "Medications",
                principalColumn: "MedicationId");
        }
    }
}
