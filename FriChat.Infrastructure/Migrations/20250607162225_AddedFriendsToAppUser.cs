using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedFriendsToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserFriendRequests",
                columns: table => new
                {
                    RequestingUserId = table.Column<int>(type: "integer", nullable: false),
                    RequestedUserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFriendRequests", x => new { x.RequestingUserId, x.RequestedUserId });
                    table.ForeignKey(
                        name: "FK_UserFriendRequests_AppUsers_RequestedUserId",
                        column: x => x.RequestedUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFriendRequests_AppUsers_RequestingUserId",
                        column: x => x.RequestingUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFriends",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    FriendId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFriends", x => new { x.UserId, x.FriendId });
                    table.ForeignKey(
                        name: "FK_UserFriends_AppUsers_FriendId",
                        column: x => x.FriendId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFriends_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserFriendRequests_RequestedUserId",
                table: "UserFriendRequests",
                column: "RequestedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFriends_FriendId",
                table: "UserFriends",
                column: "FriendId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFriendRequests");

            migrationBuilder.DropTable(
                name: "UserFriends");
        }
    }
}
