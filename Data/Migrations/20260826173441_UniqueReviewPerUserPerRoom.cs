using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicRoomBooking.Migrations
{
    /// <inheritdoc />
    public partial class UniqueReviewPerUserPerRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_RoomId",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RoomId_UserId",
                table: "Reviews",
                columns: new[] { "RoomId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_RoomId_UserId",
                table: "Reviews");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RoomId",
                table: "Reviews",
                column: "RoomId");
        }
    }
}
