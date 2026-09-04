using Microsoft.AspNetCore.Mvc.Rendering;
using mvc2025TermProject.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc2025TermProject.Models.ViewModels
{

    //RecipeCreateViewModel used only in RecipesController
    public class RecipeCreateViewModel
    {
        public int? RecipeID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Recipe Name")]
        public string? RecipeName { get; set; }

        [Display(Name = "Author")]
        public int? UserID { get; set; }
        public virtual RecipeUser? CreatedBy { get; set; }

        [Required]
        [Display(Name = "Status")]
        public Status? Status
        {
            get => Enums.Status.Draft;
        }

        [StringLength(2000)]
        [Display(Name = "Instructions")]
        public string? Instruction { get; set; }

        [Display(Name = "Prep.Time")]
        [Required(ErrorMessage = "Preparation Time is required (in min)")]
        public int? PreparationTime { get; set; }

        [Display(Name = "Cook. Time")]
        public int? CookingTime { get; set; }

        [Required]
        [Display(Name = "Servings")]
        public int? NumberOfServings { get; set; }

        [Display(Name = "Category")]
        public int? CategoryID { get; set; }

        public virtual Category? Category { get; set; }

        [Display(Name = "Ingredients")]
        public ICollection<RecipeIngredientDetail>? RecipeIngredientDetails { get; set; }

        [Display(Name = "Image")]
        public virtual ICollection<Image>? Images { get; set; }
    }

    public class RecipeViewModel
    {
        public int? RecipeID { get; set; }

        [Display(Name = "Name")]
        public string? RecipeName { get; set; }

        public Status? Status { get; set; }

        public Category? Category { get; set; }

        [Display(Name = "Created On")]
        [DataType(DataType.Date)]
        public DateTime? Created { get; set; }

        public int? PreparationTime { get; set; }

        [Display(Name = "Image")]
        public virtual ICollection<Image>? Images { get; set; }
    }

    public class StepOne
    {
        [Required]
        [Display(Name = "Category")]
        public int? CategoryID { get; set; }
        public virtual Category? Category { get; set; }

        public int? RecipeID { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string? RecipeName { get; set; }

        public int? CreatedById { get; set; }

        [Display(Name = "Created By")]
        public virtual RecipeUser? CreatedBy { get; set; }

    }

    public class IngredientSelectionViewModel
    {
        public int? IngredientID { get; set; }
        public string? IngredientName { get; set; }

        public decimal? Amount { get; set; }
        public MeasurementType? MeasurementType { get; set; }

        public bool Selected { get; set; }
    }

    public class StepTwo
    {
        [Required]
        public int? RecipeID { get; set; }

        [Display(Name = "Recipe Name")]
        public string? RecipeName { get; set; }

        [Display(Name = "Category")]
        public string? CategoryName { get; set; }

        public int? CreatedById { get; set; }

        [Display(Name = "Created By")]
        public virtual RecipeUser? CreatedBy { get; set; }
        public Status? Status { get; }

        public int SelectedIngredientID { get; set; }
        public decimal? NewAmount { get; set; }
        public MeasurementType? NewMeasurementType { get; set; }
        public List<SelectListItem>? IngredientList { get; set; }
        public List<IngredientSelectionViewModel>? Ingredients { get; set; }
    }

    public class StepThree
    {
        [Required]
        public int? RecipeID { get; set; }

        [Display(Name = "Recipe Name")]
        public string? RecipeName { get; set; }

        [Display(Name = "Category")]
        public int? CategoryID { get; set; }
        public virtual Category? Category { get; set; }

        public string? CategoryName { get; set; }

        public int? CreatedById { get; set; }

        [Display(Name = "Created By")]
        public virtual RecipeUser? CreatedBy { get; set; }

        [Display(Name = "Status")]
        public Status? Status
        {
            get => Enums.Status.Draft;
        }

        [Display(Name = "Ingredients")]
        public ICollection<RecipeIngredientDetail>? RecipeIngredientDetails { get; set; }

        public ICollection<IngredientSelectionViewModel>? Ingredients { get; set; }


        [Display(Name = "Image")]
        public virtual ICollection<Image>? Images { get; set; }

        [StringLength(2000)]
        [Display(Name = "Instructions")]
        public string? Instruction { get; set; }

        [StringLength(500)]
        [Display(Name = "Tips")]
        public string? Tips { get; set; }

        [Required]
        [Display(Name = "Prep.Time")]
        [Range(0, int.MaxValue, ErrorMessage = "Time cannot be negative.")]
        public int? PreparationTime { get; set; }

        [Display(Name = "Cook. Time")]
        [Range(0, int.MaxValue, ErrorMessage = "Time cannot be negative.")]
        public int? CookingTime { get; set; }
        
        [Display(Name = "Temp.")]
        [Range(0, 500, ErrorMessage = "Provide a value between 0 - 500 °F")]
        public int? CookingTemperature { get; set; }

        [Required]
        [Display(Name = "Servings")]
        public int? NumberOfServings { get; set; }

        [Display(Name = "Created On")]
        [DataType(DataType.Date)]
        public DateTime? Created { get; set; }

        [StringLength(100)]
        [Display(Name = "Special Equipment")]
        public string? SpecialEquipment { get; set; }

        [Display(Name = "Related Youtube Links")]
        public string? YoutubeLinks { get; set; }

        [Display(Name = "Nutritional Information")]
        [StringLength(2000)]
        public string? NutritionalInfo { get; set; }
    }
    
    public class ShareRecipeViewModel
    {
        public int? RecipeID { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Recipient Email")]
        public string? RecipientEmail { get; set; } = string.Empty;

        [Display(Name = "Subject")]
        public string? Subject { get; set; }

        [Display(Name = "Message")]
        public string? Message { get; set; }
    }
}
