using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityLib.Migrations
{
    public partial class DogCheckInCompositePrimaryKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DogCheckIns",
                table: "DogCheckIns");

            migrationBuilder.DropIndex(
                name: "IX_DogCheckIns_DogId",
                table: "DogCheckIns");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DogCheckIns",
                table: "DogCheckIns",
                columns: new[] { "DogId", "CheckInId" });

            migrationBuilder.CreateIndex(
                name: "IX_DogCheckIns_CheckInId",
                table: "DogCheckIns",
                column: "CheckInId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DogCheckIns",
                table: "DogCheckIns");

            migrationBuilder.DropIndex(
                name: "IX_DogCheckIns_CheckInId",
                table: "DogCheckIns");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DogCheckIns",
                table: "DogCheckIns",
                column: "CheckInId");

            migrationBuilder.CreateIndex(
                name: "IX_DogCheckIns_DogId",
                table: "DogCheckIns",
                column: "DogId");
        }
    }
}
