using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFilesToMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CHATFILES",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    FILE_NAME = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FILE_URL = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CONTENT_TYPE = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    FILE_SIZE = table.Column<long>(type: "INTEGER", nullable: true),
                    UPLOADED_AT = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ChatMessageId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHATFILES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CHATFILES_ChatMessages_ChatMessageId",
                        column: x => x.ChatMessageId,
                        principalTable: "ChatMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CHATFILES_ChatMessageId",
                table: "CHATFILES",
                column: "ChatMessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CHATFILES");
        }
    }
}
