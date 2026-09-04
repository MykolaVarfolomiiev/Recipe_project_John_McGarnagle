using System.ComponentModel.DataAnnotations;

namespace mvc2025TermProject.Models.ViewModels
{
    public class CategoryCreateViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string? Name { get; set; }

        [StringLength(300)]
        public string? Description { get; set; }

        public int? CreatedById { get; set; }
    }

    public class CategoryEditViewModel
    {
        public int? CategoryID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string? Name { get; set; }

        [Display(Name = "Name")]
        public string? ApprovedName { get; set; }

        [StringLength(300)]
        public string? Description { get; set; }

        [Display(Name = "Description")]
        public string? ApprovedDescription { get; set; }        

        public int? CreatedById { get; set; }
    }
}
