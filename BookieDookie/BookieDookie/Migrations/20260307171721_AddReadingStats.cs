using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookieDookie.Migrations
{
    public partial class AddReadingStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReadingStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),

                    UserId = table.Column<Guid>(nullable: false),

                    PagesReadToday = table.Column<int>(nullable: false),

                    TotalPagesRead = table.Column<int>(nullable: false),

                    BooksRead = table.Column<int>(nullable: false),

                    ReadingStreak = table.Column<int>(nullable: false),

                    Feeling = table.Column<string>(nullable: true),

                    BookmarkBook = table.Column<string>(nullable: true),

                    BookmarkPage = table.Column<int>(nullable: false),

                    LastUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingStats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReadingStats_UserId",
                table: "ReadingStats",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReadingStats");
        }
    }
}