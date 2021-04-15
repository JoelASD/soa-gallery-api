using Microsoft.EntityFrameworkCore.Migrations;

namespace SOAImageGalleryAPI.Migrations
{
    public partial class addfavorites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserHasFavourite_AspNetUsers_UserID",
                table: "UserHasFavourite");

            migrationBuilder.DropForeignKey(
                name: "FK_UserHasFavourite_Images_ImageID",
                table: "UserHasFavourite");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserHasFavourite",
                table: "UserHasFavourite");

            migrationBuilder.RenameTable(
                name: "UserHasFavourite",
                newName: "Favorites");

            migrationBuilder.RenameIndex(
                name: "IX_UserHasFavourite_UserID",
                table: "Favorites",
                newName: "IX_Favorites_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_UserHasFavourite_ImageID",
                table: "Favorites",
                newName: "IX_Favorites_ImageID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Favorites",
                table: "Favorites",
                column: "FavouriteID");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_AspNetUsers_UserID",
                table: "Favorites",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Images_ImageID",
                table: "Favorites",
                column: "ImageID",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_AspNetUsers_UserID",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Images_ImageID",
                table: "Favorites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Favorites",
                table: "Favorites");

            migrationBuilder.RenameTable(
                name: "Favorites",
                newName: "UserHasFavourite");

            migrationBuilder.RenameIndex(
                name: "IX_Favorites_UserID",
                table: "UserHasFavourite",
                newName: "IX_UserHasFavourite_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Favorites_ImageID",
                table: "UserHasFavourite",
                newName: "IX_UserHasFavourite_ImageID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserHasFavourite",
                table: "UserHasFavourite",
                column: "FavouriteID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserHasFavourite_AspNetUsers_UserID",
                table: "UserHasFavourite",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserHasFavourite_Images_ImageID",
                table: "UserHasFavourite",
                column: "ImageID",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
