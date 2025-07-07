using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FriChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserMediaAndAddedItFKtoMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentUrl",
                table: "Messages");

            migrationBuilder.AlterColumn<int>(
                name: "ConversationId",
                table: "Messages",
                type: "integer",
                nullable: false,
                comment: "Identifier for the conversation to which this message belongs",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "UserMediaId",
                table: "Messages",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "Identifier for the UserMedia to which belongs");

            migrationBuilder.CreateTable(
                name: "UserMedia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false, comment: "Unique identifier for the user media")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false, comment: "URL of the media file uploaded by the user"),
                    Type = table.Column<int>(type: "integer", nullable: false, comment: "Type of the media file (e.g., image, video)"),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "Timestamp when the media was uploaded"),
                    UserId = table.Column<int>(type: "integer", nullable: false, comment: "Identifier for the user who uploaded the media"),
                    ConversationId = table.Column<int>(type: "integer", nullable: false, comment: "Identifier for the conversation associated with the media")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMedia_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMedia_Conversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserMediaId",
                table: "Messages",
                column: "UserMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMedia_ConversationId",
                table: "UserMedia",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMedia_UserId",
                table: "UserMedia",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_UserMedia_UserMediaId",
                table: "Messages",
                column: "UserMediaId",
                principalTable: "UserMedia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_UserMedia_UserMediaId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "UserMedia");

            migrationBuilder.DropIndex(
                name: "IX_Messages_UserMediaId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "UserMediaId",
                table: "Messages");

            migrationBuilder.AlterColumn<int>(
                name: "ConversationId",
                table: "Messages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Identifier for the conversation to which this message belongs");

            migrationBuilder.AddColumn<string>(
                name: "AttachmentUrl",
                table: "Messages",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                comment: "URL of the attachment, if any (e.g., image, file)");
        }
    }
}
