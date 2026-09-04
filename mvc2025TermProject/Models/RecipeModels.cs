using mvc2025TermProject.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc2025TermProject.Models
{
    public class Recipe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? RecipeID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Recipe Name")]
        public string? RecipeName { get; set; }

        [StringLength(2000)]
        [Display(Name = "Instructions")]
        public string? Instruction { get; set; }

        [StringLength(200)]
        [Display(Name = "Tips")]
        public string? Tips { get; set; }

        [Display(Name = "Prep. Time")]
        public int? PreparationTime { get; set; }

        [Display(Name = "Cook Time")]
        public int? CookingTime { get; set; }

        [Display(Name = "Temp.")]
        public int? CookingTemperature { get; set; }

        [Display(Name = "Servings")]
        public int? NumberOfServings { get; set; }

        [Display(Name = "Category")]
        public int? CategoryID { get; set; }

        public virtual Category? Category { get; set; }

        [Display(Name = "Ingredients")]
        public virtual ICollection<RecipeIngredientDetail>? RecipeIngredientDetails { get; set; }

        [Display(Name = "Status")]
        public Status? Status { get; set; }

        [Display(Name = "Images")]
        public virtual ICollection<Image>? Images { get; set; }

        public int? CreatedById { get; set; }

        [Display(Name = "Created By")]
        public virtual RecipeUser? CreatedBy { get; set; }

        [Display(Name = "Created On")]
        [DataType(DataType.Date)]
        public DateTime? Created { get; set; }

        [StringLength(100)]
        public string? SpecialEquipment { get; set; }

        public string? YoutubeLinks { get; set; }

        [StringLength(2000)]
        public string? NutritionalInfo { get; set; }
    }

    public class Report
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? ReportID { get; set; }   

        [Required]
        public int? RecipeID { get; set; }

        [Display(Name = "Full Name")]
        [StringLength(100)]
        public string? FullName { get; set; }


        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(500)]
        public string? Reason { get; set; }
    }
}
