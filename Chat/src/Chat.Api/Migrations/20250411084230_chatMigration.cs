using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Api.Migrations
{
    /// <inheritdoc />
    public partial class chatMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    SENDER_ID = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RECEIVER_ID = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TEXT = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IS_SEEN = table.Column<bool>(type: "INTEGER", nullable: false),
                    SEEN_AT = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IS_OWN = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");
        }
    }
}
