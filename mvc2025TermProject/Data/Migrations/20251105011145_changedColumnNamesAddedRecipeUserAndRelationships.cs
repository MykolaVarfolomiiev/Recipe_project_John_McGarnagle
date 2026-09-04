using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvc2025TermProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class changedColumnNamesAddedRecipeUserAndRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "UpdateType",
                table: "Ingredients",
                newName: "ApprovedIngredientType");

            migrationBuilder.RenameColumn(
                name: "UpdateName",
                table: "Ingredients",
                newName: "ApprovedIngredientName");

            migrationBuilder.RenameColumn(
                name: "UpdateDetails",
                table: "Ingredients",
                newName: "PendingIngredientDetails");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Ingredients",
                newName: "PendingIngredientType");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Ingredients",
                newName: "PendingIngredientName");

            migrationBuilder.RenameColumn(
                name: "Details",
                table: "Ingredients",
                newName: "ApprovedIngredientDetails");

            migrationBuilder.RenameColumn(
                name: "UpdateName",
                table: "Categories",
                newName: "ApprovedCategoryName");

            migrationBuilder.RenameColumn(
                name: "UpdateDescription",
                table: "Categories",
                newName: "PendingCategoryDescription");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Categories",
                newName: "PendingCategoryName");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Categories",
                newName: "ApprovedCategoryDescription");

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Ingredients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RecipeUsers",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentityUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeUsers", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_RecipeUsers_AspNetUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_CreatedById",
                table: "Ingredients",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CreatedById",
                table: "Categories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeUsers_IdentityUserId",
                table: "RecipeUsers",
                column: "IdentityUserId",
                unique: true,
                filter: "[IdentityUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_RecipeUsers_CreatedById",
                table: "Categories",
                column: "CreatedById",
                principalTable: "RecipeUsers",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_RecipeUsers_CreatedById",
                table: "Ingredients",
                column: "CreatedById",
                principalTable: "RecipeUsers",
                principalColumn: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_RecipeUsers_CreatedById",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_RecipeUsers_CreatedById",
                table: "Ingredients");

            migrationBuilder.DropTable(
                name: "RecipeUsers");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_CreatedById",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CreatedById",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "PendingIngredientType",
                table: "Ingredients",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "PendingIngredientName",
                table: "Ingredients",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "PendingIngredientDetails",
                table: "Ingredients",
                newName: "UpdateDetails");

            migrationBuilder.RenameColumn(
                name: "ApprovedIngredientType",
                table: "Ingredients",
                newName: "UpdateType");

            migrationBuilder.RenameColumn(
                name: "ApprovedIngredientName",
                table: "Ingredients",
                newName: "UpdateName");

            migrationBuilder.RenameColumn(
                name: "ApprovedIngredientDetails",
                table: "Ingredients",
                newName: "Details");

            migrationBuilder.RenameColumn(
                name: "PendingCategoryName",
                table: "Categories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "PendingCategoryDescription",
                table: "Categories",
                newName: "UpdateDescription");

            migrationBuilder.RenameColumn(
                name: "ApprovedCategoryName",
                table: "Categories",
                newName: "UpdateName");

            migrationBuilder.RenameColumn(
                name: "ApprovedCategoryDescription",
                table: "Categories",
                newName: "Description");

            migrationBuilder.AddColumn<int>(
                name: "IsApproved",
                table: "Ingredients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IsApproved",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
