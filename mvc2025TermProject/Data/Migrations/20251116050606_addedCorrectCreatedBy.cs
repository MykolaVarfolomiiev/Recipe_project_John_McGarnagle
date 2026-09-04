using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvc2025TermProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedCorrectCreatedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_RecipeUsers_CreatedByUserID",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_CreatedByUserID",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "CreatedByUserID",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Recipes",
                newName: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_CreatedById",
                table: "Recipes",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_RecipeUsers_CreatedById",
                table: "Recipes",
                column: "CreatedById",
                principalTable: "RecipeUsers",
                principalColumn: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_RecipeUsers_CreatedById",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_CreatedById",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Recipes",
                newName: "UserID");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserID",
                table: "Recipes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_CreatedByUserID",
                table: "Recipes",
                column: "CreatedByUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_RecipeUsers_CreatedByUserID",
                table: "Recipes",
                column: "CreatedByUserID",
                principalTable: "RecipeUsers",
                principalColumn: "UserID");
        }
    }
}
