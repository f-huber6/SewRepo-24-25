using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Api.Migrations
{
    /// <inheritdoc />
    public partial class FriendShipColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FriendShips",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    USER_ID = table.Column<string>(type: "TEXT", nullable: false),
                    FRIEND_ID = table.Column<string>(type: "TEXT", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendShips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FriendShips_AspNetUsers_FRIEND_ID",
                        column: x => x.FRIEND_ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FriendShips_AspNetUsers_USER_ID",
                        column: x => x.USER_ID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendShips_FRIEND_ID",
                table: "FriendShips",
                column: "FRIEND_ID");

            migrationBuilder.CreateIndex(
                name: "IX_FriendShips_USER_ID",
                table: "FriendShips",
                column: "USER_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendShips");
        }
    }
}
