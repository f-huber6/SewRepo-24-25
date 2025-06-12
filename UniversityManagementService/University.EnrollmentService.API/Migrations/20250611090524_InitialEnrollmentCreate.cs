using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University.EnrollmentService.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialEnrollmentCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    StudentId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LectureId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId_LectureId",
                table: "Enrollments",
                columns: new[] { "StudentId", "LectureId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Enrollments");
        }
    }
}
