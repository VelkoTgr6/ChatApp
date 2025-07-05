using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriChat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedTitleFromConversation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Conversations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Conversations",
                type: "text",
                nullable: false,
                defaultValue: "",
                comment: "Title of the conversation");
        }
    }
}
