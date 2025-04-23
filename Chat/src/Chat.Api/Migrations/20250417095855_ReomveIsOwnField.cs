using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Api.Migrations
{
    /// <inheritdoc />
    public partial class ReomveIsOwnField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IS_OWN",
                table: "ChatMessages");

            migrationBuilder.CreateTable(
                name: "SAVINGS_GOALS",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    USER_ID = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TITLE = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    TARGET_AMOUNT = table.Column<decimal>(type: "TEXT", nullable: false),
                    CURRENT_AMOUNT = table.Column<decimal>(type: "TEXT", nullable: false),
                    TARGET_DATE = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SAVINGS_GOALS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SAVINGS_GOALS_AspNetUsers_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SAVINGS_GOALS_USER_ID",
                table: "SAVINGS_GOALS",
                column: "USER_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SAVINGS_GOALS");

            migrationBuilder.AddColumn<bool>(
                name: "IS_OWN",
                table: "ChatMessages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
