using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AudioService.Migrations
{
    /// <inheritdoc />
    public partial class AddedUriToTheAudiofileInTheBookInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AudioUri",
                table: "BooksInfo",
                type: "varchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioUri",
                table: "BooksInfo");
        }
    }
}
