using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareDev.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtendIdentityUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "AspNetUsers",
                type: "int",
                maxLength: 100,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AllergyId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChronicConditionId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MedicationId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SurName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AllergyId",
                table: "AspNetUsers",
                column: "AllergyId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ChronicConditionId",
                table: "AspNetUsers",
                column: "ChronicConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_MedicationId",
                table: "AspNetUsers",
                column: "MedicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Allergies_AllergyId",
                table: "AspNetUsers",
                column: "AllergyId",
                principalTable: "Allergies",
                principalColumn: "AllergyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ChronicConditions_ChronicConditionId",
                table: "AspNetUsers",
                column: "ChronicConditionId",
                principalTable: "ChronicConditions",
                principalColumn: "ChronicConditionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Medications_MedicationId",
                table: "AspNetUsers",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "MedicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Allergies_AllergyId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ChronicConditions_ChronicConditionId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Medications_MedicationId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AllergyId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ChronicConditionId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_MedicationId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AllergyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ChronicConditionId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MedicationId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SurName",
                table: "AspNetUsers");
        }
    }
}
