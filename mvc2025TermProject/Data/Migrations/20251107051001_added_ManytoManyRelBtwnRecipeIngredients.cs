using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvc2025TermProject.Data.Migrations
{
    /// <inheritdoc />
    public partial class added_ManytoManyRelBtwnRecipeIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Owners_OwnerID",
                table: "Recipes");

            migrationBuilder.DropTable(
                name: "Owners");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_OwnerID",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "OwnerID",
                table: "Recipes",
                newName: "UserID");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Recipes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Instruction",
                table: "Recipes",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<int>(
                name: "CookingTime",
                table: "Recipes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CookingTemperature",
                table: "Recipes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Recipes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserID",
                table: "Recipes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tips",
                table: "Recipes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RecipeIngredientDetails",
                columns: table => new
                {
                    RecipeID = table.Column<int>(type: "int", nullable: false),
                    IngredientID = table.Column<int>(type: "int", nullable: false),
                    IngredientAmount = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    MeasurementType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeIngredientDetails", x => new { x.RecipeID, x.IngredientID });
                    table.ForeignKey(
                        name: "FK_RecipeIngredientDetails_IngredientID",
                        column: x => x.IngredientID,
                        principalTable: "Ingredients",
                        principalColumn: "IngredientID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeIngredientDetails_ResipeID",
                        column: x => x.RecipeID,
                        principalTable: "Recipes",
                        principalColumn: "RecipeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_CreatedByUserID",
                table: "Recipes",
                column: "CreatedByUserID");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredientDetails_IngredientID",
                table: "RecipeIngredientDetails",
                column: "IngredientID");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_RecipeUsers_CreatedByUserID",
                table: "Recipes",
                column: "CreatedByUserID",
                principalTable: "RecipeUsers",
                principalColumn: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_RecipeUsers_CreatedByUserID",
                table: "Recipes");

            migrationBuilder.DropTable(
                name: "RecipeIngredientDetails");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_CreatedByUserID",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "CookingTemperature",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "CreatedByUserID",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "Tips",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Recipes",
                newName: "OwnerID");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Instruction",
                table: "Recipes",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CookingTime",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    OwnerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.OwnerID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_OwnerID",
                table: "Recipes",
                column: "OwnerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Owners_OwnerID",
                table: "Recipes",
                column: "OwnerID",
                principalTable: "Owners",
                principalColumn: "OwnerID");
        }
    }
}
