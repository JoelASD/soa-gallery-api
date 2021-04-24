using Microsoft.EntityFrameworkCore.Migrations;

namespace SOAImageGalleryAPI.Migrations
{
    public partial class addedrequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Images_ImageID",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Images_ImageID",
                table: "Votes");

            migrationBuilder.AlterColumn<string>(
                name: "ImageID",
                table: "Votes",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageID",
                table: "Comments",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Images_ImageID",
                table: "Comments",
                column: "ImageID",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Images_ImageID",
                table: "Votes",
                column: "ImageID",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Images_ImageID",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Images_ImageID",
                table: "Votes");

            migrationBuilder.AlterColumn<string>(
                name: "ImageID",
                table: "Votes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ImageID",
                table: "Comments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Images_ImageID",
                table: "Comments",
                column: "ImageID",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Images_ImageID",
                table: "Votes",
                column: "ImageID",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
