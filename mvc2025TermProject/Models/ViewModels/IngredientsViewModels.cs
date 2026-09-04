using System.ComponentModel.DataAnnotations;

namespace mvc2025TermProject.Models.ViewModels
{
    public class IngredientsCreateViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string? Name { get; set; }

        [Required]
        [StringLength(30)]
        public string? Type { get; set; }

        [StringLength(300)]
        public string? Details { get; set; }

        public int? CreatedById { get; set; }
    }

    public class IngredientsEditViewModel
    {
        public int? IngredientID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string? Name { get; set; }

        [Display(Name = "Name")]
        public string? ApprovedName { get; set; }

        [Required]
        [StringLength(30)]
        public string? Type { get; set; }

        [Display(Name = "Type")]
        public string? ApprovedType { get; set; }

        [StringLength(300)]
        public string? Details { get; set; }

        [Display(Name = "Details")]
        public string? ApprovedDetails { get; set; }

        public int? CreatedById { get; set; }
    }
}
