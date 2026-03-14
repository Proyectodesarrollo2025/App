using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FAFS.Migrations
{
    /// <inheritdoc />
    public partial class Add_FavoriteDestinations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoriteDestinations",
                schema: "Abp",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteDestinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteDestinations_Destination_DestinationId",
                        column: x => x.DestinationId,
                        principalSchema: "Abp",
                        principalTable: "Destination",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteDestinations_DestinationId",
                schema: "Abp",
                table: "FavoriteDestinations",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteDestinations_UserId_DestinationId",
                schema: "Abp",
                table: "FavoriteDestinations",
                columns: new[] { "UserId", "DestinationId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteDestinations",
                schema: "Abp");
        }
    }
}
