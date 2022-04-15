using Microsoft.EntityFrameworkCore.Migrations;

namespace Northwind.Services.EntityFrameworkCore.Blogging.Migrations
{
    public partial class AddedNavigationProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Articles_BlogArticleEntityArticleId",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "BlogArticleEntityArticleId",
                table: "Comments",
                newName: "ArticleId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_BlogArticleEntityArticleId",
                table: "Comments",
                newName: "IX_Comments_ArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Articles_ArticleId",
                table: "Comments",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Article_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Articles_ArticleId",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "ArticleId",
                table: "Comments",
                newName: "BlogArticleEntityArticleId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ArticleId",
                table: "Comments",
                newName: "IX_Comments_BlogArticleEntityArticleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Articles_BlogArticleEntityArticleId",
                table: "Comments",
                column: "BlogArticleEntityArticleId",
                principalTable: "Articles",
                principalColumn: "Article_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
