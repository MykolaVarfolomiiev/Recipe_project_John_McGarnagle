using mvc2025TermProject.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc2025TermProject.Models
{
    public class Ingredient
    {
        //CREATE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? IngredientID { get; set; }

        [Display(Name = "Approved Name")]
        [StringLength(50, MinimumLength = 2)]
        public string? ApprovedIngredientName { get; set; }

        [Display(Name = "Approved Type")]
        [StringLength(30)]
        public string? ApprovedIngredientType { get; set; }

        [Display(Name = "Approved Details")]
        [StringLength(300)]
        public string? ApprovedIngredientDetails { get; set; }

        //UPDATE

        [Display(Name = "Pending Name")]
        [StringLength(50, MinimumLength = 2)]
        public string? PendingIngredientName { get; set; }

        [Display(Name = "Pending Type")]
        [StringLength(30)]
        public string? PendingIngredientType { get; set; }

        [Display(Name = "Pending Details")]
        [StringLength(300)]
        public string? PendingIngredientDetails { get; set; }

        //RELATIONSHIPS
        public int? CreatedById { get; set; }

        [Display(Name = "Created By")]
        public virtual RecipeUser? CreatedBy { get; set; }

        //public virtual ICollection<RecipeUser>? UsersToNotify { get; set; }

        //many-to-many relationships with recipe
        public virtual ICollection<RecipeIngredientDetail>? RecipeIngredientDetails { get; set; }
    }

    public class Category
    {
        //CREATE

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? CategoryID { get; set; }

        [Display(Name = "Approved Name")]
        [StringLength(50, MinimumLength = 2)]
        public string? ApprovedCategoryName { get; set; }

        [Display(Name = "Approved Description")]
        [StringLength(300)]
        public string? ApprovedCategoryDescription { get; set; }

        //UPDATE

        [Display(Name = "Pending Name")]
        [StringLength(50, MinimumLength = 2)]
        public string? PendingCategoryName { get; set; }

        [Display(Name = "Pending Description")]
        [StringLength(300)]
        public string? PendingCategoryDescription { get; set; }

        //RELATIONSHIPS

        public int? CreatedById { get; set; }

        [Display(Name = "Created By")]
        public virtual RecipeUser? CreatedBy { get; set; }
        
        //public virtual ICollection<RecipeUser>? UsersToNotify { get; set; }

        public virtual ICollection<Recipe>? Recipes { get; set; }
    }
}
