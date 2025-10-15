using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareDev.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedBedandAdmissionsRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_Beds_BedId1",
                table: "Admissions");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_BedId1",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "BedId1",
                table: "Admissions");

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions",
                column: "BedId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions");

            migrationBuilder.AddColumn<int>(
                name: "BedId1",
                table: "Admissions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions",
                column: "BedId",
                unique: true,
                filter: "[BedId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_BedId1",
                table: "Admissions",
                column: "BedId1",
                unique: true,
                filter: "[BedId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_Beds_BedId1",
                table: "Admissions",
                column: "BedId1",
                principalTable: "Beds",
                principalColumn: "BedId");
        }
    }
}
