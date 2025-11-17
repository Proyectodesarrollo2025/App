using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FAFS.Migrations
{
    /// <inheritdoc />
    public partial class addDestinationRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DestinationRatings",
                schema: "Abp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestinationRatings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DestinationRatings_UserId_DestinationId",
                schema: "Abp",
                table: "DestinationRatings",
                columns: new[] { "UserId", "DestinationId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DestinationRatings",
                schema: "Abp");
        }
    }
}
