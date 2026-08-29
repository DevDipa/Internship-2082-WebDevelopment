using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookieDookie.Migrations
{
    /// <inheritdoc />
    public partial class AddLastReadingDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastReadingDate",
                table: "ReadingStats",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastReadingDate",
                table: "ReadingStats");
        }
    }
}
