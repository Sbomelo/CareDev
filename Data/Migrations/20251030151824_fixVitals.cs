using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareDev.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixVitals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 🧹 Removed the DropTable("PatVitals") to avoid errors
            // EF will now only create the new PatsVitals table

            migrationBuilder.CreateTable(
                name: "PatsVitals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("PK_PatsVitals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatsVitals_AspNetUsers_NurseUserId",
                        column: x => x.NurseUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatsVitals_AspNetUsers_PatientUserId",
                        column: x => x.PatientUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatsVitals_NurseUserId",
                table: "PatsVitals",
                column: "NurseUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PatsVitals_PatientUserId",
                table: "PatsVitals",
                column: "PatientUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // If we rollback, just drop PatsVitals (no need to recreate PatVitals again)
            migrationBuilder.DropTable(
                name: "PatsVitals");
        }
    }
}
