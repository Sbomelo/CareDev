using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareDev.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeBedIndexFilterBYISACTIVE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the existing unique index (without filter)
            migrationBuilder.DropIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions");

            // Create a new filtered unique index
            migrationBuilder.CreateIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions",
                column: "BedId",
                unique: true,
                filter: "[IsActive] = 1 AND [BedId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the filtered unique index
            migrationBuilder.DropIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions");

            // Recreate the original unfiltered index (rollback)
            migrationBuilder.CreateIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions",
                column: "BedId",
                unique: true);
        }
    }
}
