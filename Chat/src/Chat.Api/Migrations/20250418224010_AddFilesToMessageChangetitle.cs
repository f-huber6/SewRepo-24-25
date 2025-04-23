using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFilesToMessageChangetitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CHATFILES_ChatMessages_ChatMessageId",
                table: "CHATFILES");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CHATFILES",
                table: "CHATFILES");

            migrationBuilder.RenameTable(
                name: "CHATFILES",
                newName: "Chatfiles");

            migrationBuilder.RenameIndex(
                name: "IX_CHATFILES_ChatMessageId",
                table: "Chatfiles",
                newName: "IX_Chatfiles_ChatMessageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chatfiles",
                table: "Chatfiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chatfiles_ChatMessages_ChatMessageId",
                table: "Chatfiles",
                column: "ChatMessageId",
                principalTable: "ChatMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chatfiles_ChatMessages_ChatMessageId",
                table: "Chatfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chatfiles",
                table: "Chatfiles");

            migrationBuilder.RenameTable(
                name: "Chatfiles",
                newName: "CHATFILES");

            migrationBuilder.RenameIndex(
                name: "IX_Chatfiles_ChatMessageId",
                table: "CHATFILES",
                newName: "IX_CHATFILES_ChatMessageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CHATFILES",
                table: "CHATFILES",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CHATFILES_ChatMessages_ChatMessageId",
                table: "CHATFILES",
                column: "ChatMessageId",
                principalTable: "ChatMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
