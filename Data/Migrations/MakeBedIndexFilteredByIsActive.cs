using Microsoft.EntityFrameworkCore.Migrations;

public partial class MakeBedIndexFilteredByIsActive : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Drop the existing index
        migrationBuilder.DropIndex(
            name: "IX_Admissions_BedId",
            table: "Admissions");

        // Create a filtered unique index that only applies to active admissions
        migrationBuilder.CreateIndex(
            name: "IX_Admissions_BedId",
            table: "Admissions",
            column: "BedId",
            unique: true,
            filter: "[IsActive] = 1 AND [BedId] IS NOT NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Drop the filtered index
        migrationBuilder.DropIndex(
            name: "IX_Admissions_BedId",
            table: "Admissions");

        // Recreate the previous (non-filtered) index if that was the original
        migrationBuilder.CreateIndex(
            name: "IX_Admissions_BedId",
            table: "Admissions",
            column: "BedId",
            unique: true);
    }
}
