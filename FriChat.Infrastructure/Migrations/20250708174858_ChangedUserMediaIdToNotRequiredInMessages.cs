using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedUserMediaIdToNotRequiredInMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_UserMedia_UserMediaId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMedia_AppUsers_UserId",
                table: "UserMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMedia_Conversations_ConversationId",
                table: "UserMedia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMedia",
                table: "UserMedia");

            migrationBuilder.RenameTable(
                name: "UserMedia",
                newName: "UserMedias");

            migrationBuilder.RenameIndex(
                name: "IX_UserMedia_UserId",
                table: "UserMedias",
                newName: "IX_UserMedias_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserMedia_ConversationId",
                table: "UserMedias",
                newName: "IX_UserMedias_ConversationId");

            migrationBuilder.AlterColumn<int>(
                name: "UserMediaId",
                table: "Messages",
                type: "integer",
                nullable: true,
                comment: "Identifier for the UserMedia to which belongs",
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Identifier for the UserMedia to which belongs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMedias",
                table: "UserMedias",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_UserMedias_UserMediaId",
                table: "Messages",
                column: "UserMediaId",
                principalTable: "UserMedias",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMedias_AppUsers_UserId",
                table: "UserMedias",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMedias_Conversations_ConversationId",
                table: "UserMedias",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_UserMedias_UserMediaId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMedias_AppUsers_UserId",
                table: "UserMedias");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMedias_Conversations_ConversationId",
                table: "UserMedias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMedias",
                table: "UserMedias");

            migrationBuilder.RenameTable(
                name: "UserMedias",
                newName: "UserMedia");

            migrationBuilder.RenameIndex(
                name: "IX_UserMedias_UserId",
                table: "UserMedia",
                newName: "IX_UserMedia_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserMedias_ConversationId",
                table: "UserMedia",
                newName: "IX_UserMedia_ConversationId");

            migrationBuilder.AlterColumn<int>(
                name: "UserMediaId",
                table: "Messages",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "Identifier for the UserMedia to which belongs",
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true,
                oldComment: "Identifier for the UserMedia to which belongs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMedia",
                table: "UserMedia",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_UserMedia_UserMediaId",
                table: "Messages",
                column: "UserMediaId",
                principalTable: "UserMedia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMedia_AppUsers_UserId",
                table: "UserMedia",
                column: "UserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMedia_Conversations_ConversationId",
                table: "UserMedia",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
