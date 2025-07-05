using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedConversationImageAndName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConversationImageUrl",
                table: "Conversations",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                comment: "URL of the conversation image, if any (e.g., group chat image)");

            migrationBuilder.AddColumn<string>(
                name: "ConversationName",
                table: "Conversations",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConversationImageUrl",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "ConversationName",
                table: "Conversations");
        }
    }
}
