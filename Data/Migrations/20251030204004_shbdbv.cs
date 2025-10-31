using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareDev.Data.Migrations
{
    /// <inheritdoc />
    public partial class shbdbv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorInstructions_Patients_PatientId",
                table: "DoctorInstructions");

            migrationBuilder.DropIndex(
                name: "IX_DoctorInstructions_PatientId",
                table: "DoctorInstructions");

            migrationBuilder.AlterColumn<string>(
                name: "PatientId",
                table: "DoctorInstructions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "PatientId1",
                table: "DoctorInstructions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoctorInstructions_PatientId1",
                table: "DoctorInstructions",
                column: "PatientId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorInstructions_Patients_PatientId1",
                table: "DoctorInstructions",
                column: "PatientId1",
                principalTable: "Patients",
                principalColumn: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorInstructions_Patients_PatientId1",
                table: "DoctorInstructions");

            migrationBuilder.DropIndex(
                name: "IX_DoctorInstructions_PatientId1",
                table: "DoctorInstructions");

            migrationBuilder.DropColumn(
                name: "PatientId1",
                table: "DoctorInstructions");

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "DoctorInstructions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorInstructions_PatientId",
                table: "DoctorInstructions",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorInstructions_Patients_PatientId",
                table: "DoctorInstructions",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
