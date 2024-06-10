using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KursavayaECS.Migrations
{
    /// <inheritdoc />
    public partial class addedIndexForCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_Email_ID",
                table: "Users",
                columns: new[] { "Email", "ID" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email_ID",
                table: "Users");
        }
    }
}
