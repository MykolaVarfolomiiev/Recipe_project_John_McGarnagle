using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc2025TermProject.Models
{
    public class RecipeUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? MiddleName { get; set; }

        [Required]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string? StreetAddress { get; set; }

        [Required]
        [StringLength(50)]
        public string? Municipality { get; set; }

        [Required]
        [Phone]
        public string? PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        //[EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please enter a valid email address")]
        public string? EmailAddress { get; set; }

        [Required]
        public string? UserName { get; set; }

        //RELATIONSHIP

        public string? IdentityUserId { get; set; }
        public virtual IdentityUser? IdentityUser { get; set; }

        public virtual ICollection<Ingredient>? Ingredients { get; set; } = new List<Ingredient>();

        public virtual ICollection<Category>? Categories { get; set; } = new List<Category>();

        public virtual ICollection<Recipe>? Recipes { get; set; }
    }
}
