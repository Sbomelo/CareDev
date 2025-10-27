using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CareDev.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedSihklesChangesAndChangedBedNumberLenght : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropForeignKey(
                name: "FK_PatientVitals_User_NurseUserId",
                table: "PatientVitals");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientVitals_User_PatientUserId",
                table: "PatientVitals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_PatientVitals_NurseUserId",
                table: "PatientVitals");

            migrationBuilder.DropIndex(
                name: "IX_PatientVitals_PatientUserId",
                table: "PatientVitals");

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
                name: "Id",
                table: "User",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "PatientUserId",
                table: "PatientVitals",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "NurseUserId",
                table: "PatientVitals",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<long>(
                name: "NurseUserId1",
                table: "PatientVitals",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "PatientUserId1",
                table: "PatientVitals",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "BedNumber",
                table: "Beds",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "UserId");

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
                name: "IX_PatientVitals_NurseUserId1",
                table: "PatientVitals",
                column: "NurseUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVitals_PatientUserId1",
                table: "PatientVitals",
                column: "PatientUserId1");

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

            migrationBuilder.AddForeignKey(
                name: "FK_PatientVitals_User_NurseUserId1",
                table: "PatientVitals",
                column: "NurseUserId1",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientVitals_User_PatientUserId1",
                table: "PatientVitals",
                column: "PatientUserId1",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Roles_RoleId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientMovements_AspNetUsers_MovedByUserId",
                table: "PatientMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientVitals_User_NurseUserId1",
                table: "PatientVitals");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientVitals_User_PatientUserId1",
                table: "PatientVitals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_PatientVitals_NurseUserId1",
                table: "PatientVitals");

            migrationBuilder.DropIndex(
                name: "IX_PatientVitals_PatientUserId1",
                table: "PatientVitals");

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

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "NurseUserId1",
                table: "PatientVitals");

            migrationBuilder.DropColumn(
                name: "PatientUserId1",
                table: "PatientVitals");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "User",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PatientUserId",
                table: "PatientVitals",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NurseUserId",
                table: "PatientVitals",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BedNumber",
                table: "Beds",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "BedId1",
                table: "Admissions",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVitals_NurseUserId",
                table: "PatientVitals",
                column: "NurseUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVitals_PatientUserId",
                table: "PatientVitals",
                column: "PatientUserId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_PatientVitals_User_NurseUserId",
                table: "PatientVitals",
                column: "NurseUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientVitals_User_PatientUserId",
                table: "PatientVitals",
                column: "PatientUserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
