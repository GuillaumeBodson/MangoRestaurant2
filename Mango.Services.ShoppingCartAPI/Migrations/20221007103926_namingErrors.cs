using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mango.Services.ShoppingCartAPI.Migrations
{
    public partial class namingErrors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_CartHeaders_HeaderId",
                table: "CartDetails");

            migrationBuilder.RenameColumn(
                name: "HeaderId",
                table: "CartDetails",
                newName: "CartHeaderId");

            migrationBuilder.RenameIndex(
                name: "IX_CartDetails_HeaderId",
                table: "CartDetails",
                newName: "IX_CartDetails_CartHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_CartHeaders_CartHeaderId",
                table: "CartDetails",
                column: "CartHeaderId",
                principalTable: "CartHeaders",
                principalColumn: "CartHeaderId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_CartHeaders_CartHeaderId",
                table: "CartDetails");

            migrationBuilder.RenameColumn(
                name: "CartHeaderId",
                table: "CartDetails",
                newName: "HeaderId");

            migrationBuilder.RenameIndex(
                name: "IX_CartDetails_CartHeaderId",
                table: "CartDetails",
                newName: "IX_CartDetails_HeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_CartHeaders_HeaderId",
                table: "CartDetails",
                column: "HeaderId",
                principalTable: "CartHeaders",
                principalColumn: "CartHeaderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
