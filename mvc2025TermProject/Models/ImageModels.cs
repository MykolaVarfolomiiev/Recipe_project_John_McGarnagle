using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvc2025TermProject.Models
{
    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? ImageID { get; set; }

        [Display(Name = "Recipe")]
        public int? RecipeID { get; set; }
        public virtual Recipe? Recipe { get; set; }

        [Required]
        [MaxLength(500)]
        public string? FilePath { get; set; }

        [Required]
        [MaxLength(255)]
        public string? FileName { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(255)]
        public string? AltText { get; set; }

        [Display(Name = "Primary Image")]
        public bool? IsPrimary { get; set; }

        [Display(Name = "Approved")]
        public bool? IsApproved { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Created At")]
        public DateTime? CreatedAt { get; set; }

    }
}
