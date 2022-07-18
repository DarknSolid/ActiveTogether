using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EntityLib.Migrations
{
    public partial class AbstractedToPlacesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_ReviewerId",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ReviewerId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "DogParks");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "DogParks");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "DogParks");

            migrationBuilder.DropColumn(
                name: "FacilityId",
                table: "CheckIns");

            migrationBuilder.RenameColumn(
                name: "ReviewType",
                table: "Reviews",
                newName: "PlaceId");

            migrationBuilder.RenameColumn(
                name: "RevieweeId",
                table: "Reviews",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "DogParks",
                newName: "PlaceId");

            migrationBuilder.RenameColumn(
                name: "FacilityType",
                table: "CheckIns",
                newName: "PlaceId");

            migrationBuilder.AlterColumn<int>(
                name: "PlaceId",
                table: "DogParks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews",
                columns: new[] { "UserId", "PlaceId" });

            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FacilityType = table.Column<int>(type: "integer", nullable: false),
                    Location = table.Column<Point>(type: "geometry (point)", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PlaceId",
                table: "Reviews",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_PlaceId",
                table: "CheckIns",
                column: "PlaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckIns_Places_PlaceId",
                table: "CheckIns",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DogParks_Places_PlaceId",
                table: "DogParks",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Places_PlaceId",
                table: "Reviews",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckIns_Places_PlaceId",
                table: "CheckIns");

            migrationBuilder.DropForeignKey(
                name: "FK_DogParks_Places_PlaceId",
                table: "DogParks");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_AspNetUsers_UserId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Places_PlaceId",
                table: "Reviews");

            migrationBuilder.DropTable(
                name: "Places");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_PlaceId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_CheckIns_PlaceId",
                table: "CheckIns");

            migrationBuilder.RenameColumn(
                name: "PlaceId",
                table: "Reviews",
                newName: "ReviewType");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Reviews",
                newName: "RevieweeId");

            migrationBuilder.RenameColumn(
                name: "PlaceId",
                table: "DogParks",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PlaceId",
                table: "CheckIns",
                newName: "FacilityType");

            migrationBuilder.AddColumn<int>(
                name: "ReviewerId",
                table: "Reviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "DogParks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "DogParks",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Point>(
                name: "Location",
                table: "DogParks",
                type: "geometry (point)",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DogParks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FacilityId",
                table: "CheckIns",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews",
                columns: new[] { "ReviewerId", "RevieweeId", "ReviewType" });

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_AspNetUsers_ReviewerId",
                table: "Reviews",
                column: "ReviewerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
