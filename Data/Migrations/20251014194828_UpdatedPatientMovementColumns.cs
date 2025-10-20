using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareDev.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPatientMovementColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientMovements_Patients_PatientId",
                table: "PatientMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientMovements_Wards_WardId",
                table: "PatientMovements");

            migrationBuilder.DropColumn(
                name: "From",
                table: "PatientMovements");

            migrationBuilder.RenameColumn(
                name: "MovementDate",
                table: "PatientMovements",
                newName: "MovedAt");

            migrationBuilder.AlterColumn<int>(
                name: "WardId",
                table: "PatientMovements",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AdmissionId",
                table: "PatientMovements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FromBedId",
                table: "PatientMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FromWardId",
                table: "PatientMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MovedByUserId",
                table: "PatientMovements",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientId1",
                table: "PatientMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "PatientMovements",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToBedId",
                table: "PatientMovements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ToWardId",
                table: "PatientMovements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PatientMovements_AdmissionId",
                table: "PatientMovements",
                column: "AdmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMovements_FromBedId",
                table: "PatientMovements",
                column: "FromBedId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMovements_FromWardId",
                table: "PatientMovements",
                column: "FromWardId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMovements_MovedByUserId",
                table: "PatientMovements",
                column: "MovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMovements_PatientId1",
                table: "PatientMovements",
                column: "PatientId1");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMovements_ToBedId",
                table: "PatientMovements",
                column: "ToBedId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMovements_ToWardId",
                table: "PatientMovements",
                column: "ToWardId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMovements_Admissions_AdmissionId",
                table: "PatientMovements",
                column: "AdmissionId",
                principalTable: "Admissions",
                principalColumn: "AdmissionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMovements_AspNetUsers_MovedByUserId",
                table: "PatientMovements",
                column: "MovedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMovements_Beds_FromBedId",
                table: "PatientMovements",
                column: "FromBedId",
                principalTable: "Beds",
                principalColumn: "BedId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMovements_Beds_ToBedId",
                table: "PatientMovements",
                column: "ToBedId",
                principalTable: "Beds",
                principalColumn: "BedId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMovements_Patients_PatientId",
                table: "PatientMovements",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMovements_Patients_PatientId1",
                table: "PatientMovements",
                column: "PatientId1",
                principalTable: "Patients",
                principalColumn: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMovements_Wards_FromWardId",
                table: "PatientMovements",
                column: "FromWardId",
                principalTable: "Wards",
                principalColumn: "WardId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMovements_Wards_ToWardId",
                table: "PatientMovements",
                column: "ToWardId",
                principalTable: "Wards",
                principalColumn: "WardId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMovements_Wards_WardId",
                table: "PatientMovements",
                column: "WardId",
                principalTable: "Wards",
                principalColumn: "WardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientMovements_Admissions_AdmissionId",
                table: "PatientMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientMovements_AspNetUsers_MovedByUserId",
                table: "PatientMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientMovements_Beds_FromBedId",
                table: "PatientMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientMovements_Beds_ToBedId",
                table: "PatientMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientMovements_Patients_PatientId",
                table: "PatientMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientMovements_Patients_PatientId1",
                table: "PatientMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientMovements_Wards_FromWardId",
                table: "PatientMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientMovements_Wards_ToWardId",
                table: "PatientMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientMovements_Wards_WardId",
                table: "PatientMovements");

            migrationBuilder.DropIndex(
                name: "IX_PatientMovements_AdmissionId",
                table: "PatientMovements");

            migrationBuilder.DropIndex(
                name: "IX_PatientMovements_FromBedId",
                table: "PatientMovements");

            migrationBuilder.DropIndex(
                name: "IX_PatientMovements_FromWardId",
                table: "PatientMovements");

            migrationBuilder.DropIndex(
                name: "IX_PatientMovements_MovedByUserId",
                table: "PatientMovements");

            migrationBuilder.DropIndex(
                name: "IX_PatientMovements_PatientId1",
                table: "PatientMovements");

            migrationBuilder.DropIndex(
                name: "IX_PatientMovements_ToBedId",
                table: "PatientMovements");

            migrationBuilder.DropIndex(
                name: "IX_PatientMovements_ToWardId",
                table: "PatientMovements");

            migrationBuilder.DropColumn(
                name: "AdmissionId",
                table: "PatientMovements");

            migrationBuilder.DropColumn(
                name: "FromBedId",
                table: "PatientMovements");

            migrationBuilder.DropColumn(
                name: "FromWardId",
                table: "PatientMovements");

            migrationBuilder.DropColumn(
                name: "MovedByUserId",
                table: "PatientMovements");

            migrationBuilder.DropColumn(
                name: "PatientId1",
                table: "PatientMovements");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "PatientMovements");

            migrationBuilder.DropColumn(
                name: "ToBedId",
                table: "PatientMovements");

            migrationBuilder.DropColumn(
                name: "ToWardId",
                table: "PatientMovements");

            migrationBuilder.RenameColumn(
                name: "MovedAt",
                table: "PatientMovements",
                newName: "MovementDate");

            migrationBuilder.AlterColumn<int>(
                name: "WardId",
                table: "PatientMovements",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "From",
                table: "PatientMovements",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMovements_Patients_PatientId",
                table: "PatientMovements",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMovements_Wards_WardId",
                table: "PatientMovements",
                column: "WardId",
                principalTable: "Wards",
                principalColumn: "WardId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
