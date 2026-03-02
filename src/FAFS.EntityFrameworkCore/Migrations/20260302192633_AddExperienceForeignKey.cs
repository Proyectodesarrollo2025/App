using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FAFS.Migrations
{
    /// <inheritdoc />
    public partial class AddExperienceForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Experiences_Destination_DestinationId",
                schema: "Abp",
                table: "Experiences",
                column: "DestinationId",
                principalSchema: "Abp",
                principalTable: "Destination",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Experiences_Destination_DestinationId",
                schema: "Abp",
                table: "Experiences");
        }
    }
}
