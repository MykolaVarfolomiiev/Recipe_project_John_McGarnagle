using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace mvc2025TermProject.Models
{
    public class RecipeIngredientDetail
    {
        [Required]
        [Display(Name = "Recipe Name")]
        public int? RecipeID { get; set; }
        public virtual Recipe? Recipe { get; set; }
        [Required]
        [Display(Name = "Ingredient")]
        public int? IngredientID { get; set; }
        public virtual Ingredient? Ingredient { get; set; }

        [Precision(6, 2)]
        public decimal? IngredientAmount { get; set; }
        public MeasurementType? MeasurementType { get; set; }
    }

    public enum MeasurementType
    {
        Cup,
        Quarter,
        Piece,
        Kilogram,
        Gram,
        Liter,
        Milliliter,
        Pound,
        TableSpoon,
        TeaSpoon
    }
}
