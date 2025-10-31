using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareDev.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCompletedToDoctorInstruction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "DoctorInstructions",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "DoctorInstructions",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "DoctorInstructions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_DoctorInstructions_PatientId",
                table: "DoctorInstructions",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorInstructions_UserId",
                table: "DoctorInstructions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorInstructions_AspNetUsers_UserId",
                table: "DoctorInstructions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorInstructions_Patients_PatientId",
                table: "DoctorInstructions",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorInstructions_AspNetUsers_UserId",
                table: "DoctorInstructions");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorInstructions_Patients_PatientId",
                table: "DoctorInstructions");

            migrationBuilder.DropIndex(
                name: "IX_DoctorInstructions_PatientId",
                table: "DoctorInstructions");

            migrationBuilder.DropIndex(
                name: "IX_DoctorInstructions_UserId",
                table: "DoctorInstructions");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "DoctorInstructions");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "DoctorInstructions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<long>(
                name: "PatientId",
                table: "DoctorInstructions",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "PatientId1",
                table: "DoctorInstructions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DoctorInstructions_PatientId1",
                table: "DoctorInstructions",
                column: "PatientId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorInstructions_Patients_PatientId1",
                table: "DoctorInstructions",
                column: "PatientId1",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
