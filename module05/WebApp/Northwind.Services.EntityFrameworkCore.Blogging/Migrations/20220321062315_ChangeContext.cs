using Microsoft.EntityFrameworkCore.Migrations;

namespace Northwind.Services.EntityFrameworkCore.Blogging.Migrations
{
    public partial class ChangeContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Product_ID = table.Column<int>(type: "int", nullable: false),
                    Article_ID = table.Column<int>(type: "int", nullable: false),
                    BlogArticleEntityArticleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => new { x.Product_ID, x.Article_ID });
                    table.ForeignKey(
                        name: "FK_Products_Articles_BlogArticleEntityArticleId",
                        column: x => x.BlogArticleEntityArticleId,
                        principalTable: "Articles",
                        principalColumn: "Article_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_BlogArticleEntityArticleId",
                table: "Products",
                column: "BlogArticleEntityArticleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
