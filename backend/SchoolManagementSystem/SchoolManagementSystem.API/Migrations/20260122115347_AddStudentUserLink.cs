using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentUserLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Courses_CourseId1",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Courses_CourseId1",
                table: "Exams");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherCourses_Courses_CourseId1",
                table: "TeacherCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherCourses_Teachers_TeacherId1",
                table: "TeacherCourses");

            migrationBuilder.DropIndex(
                name: "IX_TeacherCourses_CourseId1",
                table: "TeacherCourses");

            migrationBuilder.DropIndex(
                name: "IX_TeacherCourses_TeacherId1",
                table: "TeacherCourses");

            migrationBuilder.DropIndex(
                name: "IX_Exams_CourseId1",
                table: "Exams");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_CourseId1",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "TeacherCourses");

            migrationBuilder.DropColumn(
                name: "TeacherId1",
                table: "TeacherCourses");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "Attendances");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityUserId",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Score",
                table: "Grades",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateTable(
                name: "IdentityUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUser", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_IdentityUserId",
                table: "Students",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_IdentityUser_IdentityUserId",
                table: "Students",
                column: "IdentityUserId",
                principalTable: "IdentityUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_IdentityUser_IdentityUserId",
                table: "Students");

            migrationBuilder.DropTable(
                name: "IdentityUser");

            migrationBuilder.DropIndex(
                name: "IX_Students_IdentityUserId",
                table: "Students");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId1",
                table: "TeacherCourses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId1",
                table: "TeacherCourses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdentityUserId",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Score",
                table: "Grades",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId1",
                table: "Exams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId1",
                table: "Attendances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherCourses_CourseId1",
                table: "TeacherCourses",
                column: "CourseId1");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherCourses_TeacherId1",
                table: "TeacherCourses",
                column: "TeacherId1");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_CourseId1",
                table: "Exams",
                column: "CourseId1");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_CourseId1",
                table: "Attendances",
                column: "CourseId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Courses_CourseId1",
                table: "Attendances",
                column: "CourseId1",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Courses_CourseId1",
                table: "Exams",
                column: "CourseId1",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherCourses_Courses_CourseId1",
                table: "TeacherCourses",
                column: "CourseId1",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherCourses_Teachers_TeacherId1",
                table: "TeacherCourses",
                column: "TeacherId1",
                principalTable: "Teachers",
                principalColumn: "Id");
        }
    }
}
