using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Models;

namespace mvc2025TermProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<RecipeUser> RecipeUsers { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Recipe> Recipes { get; set; }

        public DbSet<RecipeIngredientDetail> RecipeIngredientDetails { get; set; }

        public DbSet<Report> Reports { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RecipeUser>()
                .HasOne(ru => ru.IdentityUser)
                .WithOne()
                .HasForeignKey<RecipeUser>(r => r.IdentityUserId);

            builder.Entity<Ingredient>()
                .HasOne(i => i.CreatedBy)
                .WithMany(ru => ru.Ingredients)
                .HasForeignKey(i => i.CreatedById);

            builder.Entity<Category>()
                .HasOne(c => c.CreatedBy)
                .WithMany(ru => ru.Categories)
                .HasForeignKey(c => c.CreatedById);

            builder.Entity<Recipe>()
                .HasMany(r => r.RecipeIngredientDetails)
                .WithOne(ri => ri.Recipe)
                .HasForeignKey(ri => ri.RecipeID)
                .HasConstraintName("FK_RecipeIngredientDetails_ResipeID");

            builder.Entity<Ingredient>()
                .HasMany(ing => ing.RecipeIngredientDetails)
                .WithOne(ri => ri.Ingredient)
                .HasForeignKey(ri => ri.IngredientID)
                .HasConstraintName("FK_RecipeIngredientDetails_IngredientID");

            builder.Entity<RecipeIngredientDetail>()
                .ToTable("RecipeIngredientDetails")
                .HasKey(rid => new { rid.RecipeID, rid.IngredientID });

            builder.Entity<Recipe>()
                .HasMany(r => r.Images)
                .WithOne(i => i.Recipe)
                .HasForeignKey(i => i.RecipeID);
        }
    }
}
