using EmailServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Data;
using mvc2025TermProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace mvc2025TermProject.Controllers
{
    [Authorize]
    public class ImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private const int MAX_IMAGES_PER_RECIPE = 7;
        private readonly IEmailSender _emailSender;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public ImagesController(ApplicationDbContext context, IWebHostEnvironment environment, IEmailSender emailSender, 
            BlobServiceClient blobServiceClient, IConfiguration configuration)
        {
            _context = context;
            _environment = environment;
            _emailSender = emailSender;
            _blobServiceClient = blobServiceClient;

            _containerName = configuration["AzureBlobStorage:ContainerName"]
            ?? throw new InvalidOperationException("AzureBlobStorage:ContainerName is not configured.");
        }

        // GET: Images
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.Images.Include(i => i.Recipe);
            //return View(await applicationDbContext.ToListAsync());
            var userName = User.Identity.Name;

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account");
            }

            // Find the RecipeUser by username
            var recipeUser = await _context.RecipeUsers
                .FirstOrDefaultAsync(ru => ru.IdentityUser.UserName == userName);

            if (recipeUser == null)
            {
                TempData["ErrorMessage"] = "User profile not found.";
                return RedirectToAction("Index", "Home");
            }

            // Get only images from user's recipes
            List<Image> recipeImages = await _context.Images
                .Include(i => i.Recipe)
                .Where(i => i.Recipe.CreatedById == recipeUser.UserID)
                .ToListAsync();

            return View(recipeImages);
        }

        // GET: Images/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images
                .Include(i => i.Recipe)
                .FirstOrDefaultAsync(m => m.ImageID == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // GET: Images/Create
        [HttpGet]
        public async Task<IActionResult> Create(int? recipeId)
        {
            string? userName = User.Identity.Name;

            if (string.IsNullOrEmpty(userName))
                return RedirectToAction("Login", "Account");

            // Find the RecipeUser by username
            var recipeUser = await _context.RecipeUsers
                .FirstOrDefaultAsync(ru => ru.IdentityUser.UserName == userName);

            if (recipeUser == null)
            {
                return BadRequest("User not found in RecipeUsers table.");
            }

            var userRecipes = _context.Recipes
                .Where(r => r.CreatedById == recipeUser.UserID)
                .ToList();

            ViewData["RecipeID"] = new SelectList(userRecipes, "RecipeID", "RecipeName");

            var model = new Image
            {
                RecipeID = recipeId,
                IsPrimary = false
            };

            return View(model);

        }

        // POST: Images/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? recipeId, IFormFile? postedFile, string? description, string? altText, bool isPrimary, bool isApproved = false)
        {
            string userName = User.Identity.Name;

            if (string.IsNullOrEmpty(userName))
                return RedirectToAction("Login", "Account");
            
            RecipeUser? recipeUser = await _context.RecipeUsers
                .FirstOrDefaultAsync(ru => ru.IdentityUser.UserName == userName);

            if (recipeUser == null)
            {
                TempData["ErrorMessage"] = "User not found";
                return RedirectToAction("Create", "Images");
            }

            Recipe? recipe = await _context.Recipes.FindAsync(recipeId);
            if (recipe == null)
                return NotFound();

            List<Image> currentImages = await _context.Images
                .Where(i => i.RecipeID == recipeId)
                .ToListAsync();

            int currentImageCount = currentImages.Count;

            if (currentImageCount >= MAX_IMAGES_PER_RECIPE)
            {
                var userRecipes = _context.Recipes
                    .Where(r => r.CreatedById == recipeUser.UserID)
                    .ToList();

                ViewBag.Message = $"You cannot add more images.This recipe already has the maximum of {MAX_IMAGES_PER_RECIPE} images.";
                ViewBag.RecipeID = new SelectList(userRecipes, "RecipeID", "RecipeName", recipeId);
                ViewBag.RecipeName = recipe.RecipeName;
                return View();
            }

            if (postedFile != null)
            {
                const int fileSizeLimit = 2 * 1024 * 1024; // 2 MB

                if (postedFile.Length <= fileSizeLimit)
                {
                    string fileType = postedFile.ContentType.ToLower();
                    bool isValidType = false;

                    switch (fileType)
                    {
                        case "image/jpeg":
                        case "image/jpg":
                            ViewBag.Message = "This is a .jpg file";
                            isValidType = true;
                            break;
                        case "image/png":
                            ViewBag.Message = "This is a .png file";
                            isValidType = true;
                            break;
                        case "image/gif":
                            ViewBag.Message = "This is a .gif file";
                            isValidType = true;
                            break;
                      
                        default:
                            ViewBag.Message = $"Unsupported File Type: {fileType}. Allowed types: JPG, PNG, GIF";
                            isValidType = false;
                            break;
                    }

                    if (isValidType)
                    {
                        try
                        {
                            string extension = Path.GetExtension(postedFile.FileName);
                            string uniqueFileName = $"{Guid.NewGuid()}{extension}";

                            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

                            var blobName = recipeId.HasValue
                                ? $"{recipeId}/{uniqueFileName}"
                                : $"temp/{uniqueFileName}";

                            var blobClient = containerClient.GetBlobClient(blobName);

                            using var stream = postedFile.OpenReadStream();

                            await blobClient.UploadAsync(
                                stream,
                                new BlobHttpHeaders
                                {
                                    ContentType = postedFile.ContentType
                                });

                            string filePath = blobClient.Uri.ToString();

                            if (isApproved)
                            {
                                var msg = new EmailMessage(
                                    new[] { recipeUser.EmailAddress },
                                    "Image Approved",
                                    $"Your image {postedFile.FileName} to your {recipe.RecipeName} recipe is approved."
                                );

                                _emailSender.SendEmail(msg);
                            }

                            if (isPrimary && recipeId.HasValue)
                            {
                                var existingPrimary = await _context.Images
                                    .Where(i => i.RecipeID == recipeId && i.IsPrimary == true)
                                    .ToListAsync();

                                foreach (var img in existingPrimary)
                                {
                                    img.IsPrimary = false;
                                    _context.Update(img);
                                }
                            }

                            var image = new Image
                                {
                                    RecipeID = recipeId,
                                    FileName = postedFile.FileName,
                                    FilePath = filePath,
                                    Description = description,
                                    AltText = altText ?? postedFile.FileName,
                                    IsPrimary = isPrimary,
                                    IsApproved = isApproved,
                                    CreatedAt = DateTime.Now
                                };

                            _context.Images.Add(image);
                            await _context.SaveChangesAsync();

                            ViewBag.Message = "File was successfully uploaded and is pending approval!";
                            TempData["SuccessMessage"] = "Image uploaded successfully and is pending approval by admin.";

                            // Redirect based on context
                            if (recipeId.HasValue)
                            {
                                return RedirectToAction("MyRecipes", "Recipes1", new { id = recipeId });
                            }
                            return RedirectToAction(nameof(Index));

                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = $"Error uploading file: {ex.Message}";
                        }
                    }
                }
                else
                {
                    ViewBag.Message = $"File exceeds maximum size of {fileSizeLimit / (1024 * 1024)} MB.";
                }
            }
            else
            {
                ViewBag.Message = "Please choose a file.";
            }

            var userRecipesFinal = _context.Recipes
                .Where(r => r.CreatedById == recipeUser.UserID)
                .ToList();

            ViewBag.RecipeID = new SelectList(userRecipesFinal, "RecipeID", "RecipeName", recipeId);
            ViewBag.RecipeName = recipe.RecipeName;

            return View();
        }

        // GET: Images/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            ViewData["RecipeID"] = new SelectList(_context.Recipes, "RecipeID", "RecipeName", image.RecipeID);
            return View(image);
        }

        // POST: Images/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("ImageID,RecipeID,FilePath,FileName,Description,AltText,IsPrimary,IsApproved,CreatedAt")] Image image)
        {
            if (id != image.ImageID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(image);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImageExists(image.ImageID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RecipeID"] = new SelectList(_context.Recipes, "RecipeID", "RecipeName", image.RecipeID);
            return View(image);
        }

        // GET: Images/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images
                .Include(i => i.Recipe)
                .FirstOrDefaultAsync(m => m.ImageID == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image != null)
            {
                if (!string.IsNullOrWhiteSpace(image.FilePath))
                {
                    var blobUri = new Uri(image.FilePath);

                    string containerPrefix = $"/{_containerName}/";

                    int prefixIndex = blobUri.AbsolutePath.IndexOf(
                        containerPrefix,
                        StringComparison.OrdinalIgnoreCase);

                    if (prefixIndex >= 0)
                    {
                        string blobName = Uri.UnescapeDataString(
                            blobUri.AbsolutePath.Substring(
                                prefixIndex + containerPrefix.Length));

                        var containerClient =
                            _blobServiceClient.GetBlobContainerClient(_containerName);

                        var blobClient =
                            containerClient.GetBlobClient(blobName);

                        await blobClient.DeleteIfExistsAsync();
                    }
                }

                _context.Images.Remove(image);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // For retrieving private image from blob and show it in the browser
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Serve(int id)
        {
            var image = await _context.Images.FindAsync(id);

            if (image == null || string.IsNullOrEmpty(image.FilePath))
            {
                return NotFound();
            }

            var containerClient =
                _blobServiceClient.GetBlobContainerClient(_containerName);

            // Extract blob name from the stored Blob URL
            var blobUri = new Uri(image.FilePath);

            string containerPrefix = $"/{_containerName}/";

            int prefixIndex = blobUri.AbsolutePath.IndexOf(
                containerPrefix,
                StringComparison.OrdinalIgnoreCase);

            if (prefixIndex < 0)
            {
                return NotFound();
            }

            string blobName = Uri.UnescapeDataString(
                blobUri.AbsolutePath.Substring(
                    prefixIndex + containerPrefix.Length));

            var blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())
            {
                return NotFound();
            }

            var download = await blobClient.DownloadStreamingAsync();

            return File(
                download.Value.Content,
                download.Value.Details.ContentType ?? "application/octet-stream");
        }

        private bool ImageExists(int? id)
        {
            return _context.Images.Any(e => e.ImageID == id);
        }
    }
}
