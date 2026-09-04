namespace mvc2025TermProject.Models.ViewModels
{
    public class ImageSelectionViewModel
    {
        public int? ImageID { get; set; }
        public string? ImageName { get; set; }

        public bool IsPrimary { get; set; }
        public bool IsSelected { get; set; }
    }

    public class RecipeAddImagesViewModel
    {
        public int? RecipeID { get; set; }

        public string? RecipeName { get; set;}

        public List<ImageSelectionViewModel>? ImagesList { get; set; }
    }
}
