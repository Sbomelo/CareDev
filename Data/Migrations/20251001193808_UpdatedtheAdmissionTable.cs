using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareDev.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedtheAdmissionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_RoomTypes_RoomId1",
                table: "Admissions");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_RoomId1",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "RoomId1",
                table: "Admissions");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                table: "Admissions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BedId",
                table: "Admissions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "AdmissionReason",
                table: "Admissions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "Admissions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Admissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoomTypeRoomId",
                table: "Admissions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Doctor",
                columns: table => new
                {
                    DoctorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SurName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctor", x => x.DoctorId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions",
                column: "BedId",
                unique: true,
                filter: "[BedId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_DoctorId",
                table: "Admissions",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_RoomTypeRoomId",
                table: "Admissions",
                column: "RoomTypeRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_Doctor_DoctorId",
                table: "Admissions",
                column: "DoctorId",
                principalTable: "Doctor",
                principalColumn: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_RoomTypes_RoomTypeRoomId",
                table: "Admissions",
                column: "RoomTypeRoomId",
                principalTable: "RoomTypes",
                principalColumn: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_Doctor_DoctorId",
                table: "Admissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Admissions_RoomTypes_RoomTypeRoomId",
                table: "Admissions");

            migrationBuilder.DropTable(
                name: "Doctor");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_DoctorId",
                table: "Admissions");

            migrationBuilder.DropIndex(
                name: "IX_Admissions_RoomTypeRoomId",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Admissions");

            migrationBuilder.DropColumn(
                name: "RoomTypeRoomId",
                table: "Admissions");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                table: "Admissions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BedId",
                table: "Admissions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdmissionReason",
                table: "Admissions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoomId",
                table: "Admissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RoomId1",
                table: "Admissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_BedId",
                table: "Admissions",
                column: "BedId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admissions_RoomId1",
                table: "Admissions",
                column: "RoomId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Admissions_RoomTypes_RoomId1",
                table: "Admissions",
                column: "RoomId1",
                principalTable: "RoomTypes",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
