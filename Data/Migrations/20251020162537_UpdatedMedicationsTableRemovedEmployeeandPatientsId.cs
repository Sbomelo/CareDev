using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CareDev.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedMedicationsTableRemovedEmployeeandPatientsId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Roles_RoleId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientMovements_AspNetUsers_MovedByUserId",
                table: "PatientMovements");

            migrationBuilder.DropIndex(
                name: "IX_Beds_BedNumber_WardId",
                table: "Beds");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions");

            migrationBuilder.DeleteData(
                table: "Beds",
                keyColumn: "BedId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Beds",
                keyColumn: "BedId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Beds",
                keyColumn: "BedId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Beds",
                keyColumn: "BedId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Beds",
                keyColumn: "BedId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Beds",
                keyColumn: "BedId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Beds",
                keyColumn: "BedId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Beds",
                keyColumn: "BedId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Beds",
                keyColumn: "BedId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Wards",
                keyColumn: "WardId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Wards",
                keyColumn: "WardId",
                keyValue: 6);

            migrationBuilder.AlterColumn<string>(
                name: "BedNumber",
                table: "Beds",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "BedId1",
                table: "Admissions",
                type: "int",
                nullable: true);

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
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pictureUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "PatientVitals",
                columns: table => new
                {
                    VitalID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NurseUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Temperature = table.Column<double>(type: "float", nullable: false),
                    HeartRate = table.Column<int>(type: "int", nullable: false),
                    RespiratoryRate = table.Column<int>(type: "int", nullable: false),
                    BloodPressure = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OxygenSaturation = table.Column<int>(type: "int", nullable: false),
                    GlucoseLevel = table.Column<double>(type: "float", nullable: true),
                    RecordedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientVitals", x => x.VitalID);
                    table.ForeignKey(
                        name: "FK_PatientVitals_User_NurseUserId",
                        column: x => x.NurseUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientVitals_User_PatientUserId",
                        column: x => x.PatientUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions",
                column: "BedId",
                unique: true,
                filter: "[BedId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_BedId1",
                table: "Admissions",
                column: "BedId1");

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

            migrationBuilder.CreateIndex(
                name: "IX_PatientVitals_NurseUserId",
                table: "PatientVitals",
                column: "NurseUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVitals_PatientUserId",
                table: "PatientVitals",
                column: "PatientUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_Beds_BedId1",
                table: "Admissions",
                column: "BedId1",
                principalTable: "Beds",
                principalColumn: "BedId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Roles_RoleId",
                table: "Employees",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMovements_AspNetUsers_MovedByUserId",
                table: "PatientMovements",
                column: "MovedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_Beds_BedId1",
                table: "Admissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Roles_RoleId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientMovements_AspNetUsers_MovedByUserId",
                table: "PatientMovements");

            migrationBuilder.DropTable(
                name: "AdministeredMeds");

            migrationBuilder.DropTable(
                name: "PatientVitals");

            migrationBuilder.DropTable(
                name: "MedicationDispensations");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_BedId1",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "BedId1",
                table: "Admissions");

            migrationBuilder.AlterColumn<string>(
                name: "BedNumber",
                table: "Beds",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Beds",
                columns: new[] { "BedId", "BedNumber", "IsAvailable", "WardId" },
                values: new object[,]
                {
                    { 4, "S-201", false, 2 },
                    { 5, "S-202", false, 2 },
                    { 6, "S-203", false, 2 },
                    { 7, "M-301", false, 3 },
                    { 8, "M-302", false, 3 },
                    { 9, "M-303", false, 3 }
                });

            migrationBuilder.InsertData(
                table: "Wards",
                columns: new[] { "WardId", "Name" },
                values: new object[,]
                {
                    { 4, "X Ray Ward" },
                    { 6, "Labour Ward" }
                });

            migrationBuilder.InsertData(
                table: "Beds",
                columns: new[] { "BedId", "BedNumber", "IsAvailable", "WardId" },
                values: new object[,]
                {
                    { 1, "L-101", false, 6 },
                    { 2, "L-102", false, 6 },
                    { 3, "L-103", false, 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Beds_BedNumber_WardId",
                table: "Beds",
                columns: new[] { "BedNumber", "WardId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions",
                column: "BedId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Roles_RoleId",
                table: "Employees",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMovements_AspNetUsers_MovedByUserId",
                table: "PatientMovements",
                column: "MovedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
