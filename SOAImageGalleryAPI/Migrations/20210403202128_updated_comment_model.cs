using Microsoft.EntityFrameworkCore.Migrations;

namespace SOAImageGalleryAPI.Migrations
{
    public partial class updated_comment_model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Comment_CommentId1",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Images_ImageID",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Users_UserID",
                table: "Comment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_CommentId1",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "CommentId1",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "CommentParentID",
                table: "Comment");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_UserID",
                table: "Comments",
                newName: "IX_Comments_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_ImageID",
                table: "Comments",
                newName: "IX_Comments_ImageID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Images_ImageID",
                table: "Comments",
                column: "ImageID",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_UserID",
                table: "Comments",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Images_ImageID",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_UserID",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comment");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_UserID",
                table: "Comment",
                newName: "IX_Comment_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ImageID",
                table: "Comment",
                newName: "IX_Comment_ImageID");

            migrationBuilder.AddColumn<string>(
                name: "CommentId1",
                table: "Comment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommentParentID",
                table: "Comment",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_CommentId1",
                table: "Comment",
                column: "CommentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Comment_CommentId1",
                table: "Comment",
                column: "CommentId1",
                principalTable: "Comment",
                principalColumn: "CommentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Images_ImageID",
                table: "Comment",
                column: "ImageID",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Users_UserID",
                table: "Comment",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
