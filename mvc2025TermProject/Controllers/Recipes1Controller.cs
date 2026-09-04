using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Data;
using mvc2025TermProject.Models;
using mvc2025TermProject.Models.Enums;
using mvc2025TermProject.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmailServices;
using Microsoft.IdentityModel.Tokens;

namespace mvc2025TermProject.Controllers
{
    [Authorize]
    public class Recipes1Controller : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailSender _emailSender;

        public Recipes1Controller(ApplicationDbContext context, IWebHostEnvironment environment, IEmailSender emailSender)
        {
            _context = context;
            _environment = environment;
            _emailSender = emailSender;
        }

        [AllowAnonymous]
        // GET: All Recipes1
        [HttpGet]
        public async Task<IActionResult> Index(DateTime? dateSearch, int? categorySearch, string? keywordSearch, int? pageNumber)
        {
            var categories = await _context.Categories
               .Select(c => new
               {
                   c.CategoryID,
                   Name = c.ApprovedCategoryName ?? c.PendingCategoryName + " (pending)"
               })
               .ToListAsync();

            ViewBag.CategoryID = new SelectList(categories, "CategoryID", "Name");

            const int pageSize = 5;

            ViewData["DateSearch"] = dateSearch?.ToString("yyyy-MM-dd");
            ViewData["CategorySearch"] = categorySearch;
            ViewData["KeywordSearch"] = keywordSearch;

            var query = _context.Recipes
                .Include(r => r.Category)
                .Where(r => r.Status == Status.Public)
                .AsQueryable();

            if (dateSearch.HasValue)
                query = query.Where(r => r.Created.HasValue && r.Created.Value.Date == dateSearch.Value.Date);

            if (categorySearch.HasValue)
                query = query.Where(r => r.CategoryID == categorySearch.Value);

            if (!(String.IsNullOrWhiteSpace(keywordSearch) || String.IsNullOrEmpty(keywordSearch)))
                query = query.Where(r =>
                    r.RecipeName.Contains(keywordSearch.Trim()) ||
                    r.RecipeIngredientDetails.Any(i =>
                        (i.Ingredient.ApprovedIngredientName)
                            .Contains(keywordSearch.Trim())
                    )
                );

            query = query.OrderBy(r => r.RecipeName);

            if (pageNumber == null)
                pageNumber = 1;

            var paginatedRecipes = await PaginatedList<RecipeViewModel>.CreateAsync(query.Select(r => new RecipeViewModel
            {
                RecipeID = r.RecipeID,
                RecipeName = r.RecipeName,
                Category = r.Category,
                Created = r.Created,
                Images = r.Images
            }),
            (int) pageNumber, 
            pageSize
            );

            return View(paginatedRecipes);
        }

        // GET: Recipes1/MyRecipes
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyRecipes(int? pageNumber)
        {
            var currentUserEmail = User.Identity?.Name;

            if (string.IsNullOrEmpty(currentUserEmail))
                return Unauthorized();

            var currentUser = await _context.RecipeUsers
                .FirstOrDefaultAsync(u => u.EmailAddress == currentUserEmail);

            if (currentUser == null)
                return Unauthorized();

            const int pageSize = 5;

            var query = _context.Recipes
                .Include(r => r.Category)
                .Where(r => r.CreatedById == currentUser.UserID)
                .OrderBy(r => r.RecipeName);

            if (pageNumber == null)
                pageNumber = 1;

            var paginatedRecipes = await PaginatedList<RecipeViewModel>.CreateAsync(
                query.Select(r => new RecipeViewModel
                {
                    RecipeID = r.RecipeID,
                    RecipeName = r.RecipeName,
                    Category = r.Category,
                    Created = r.Created,
                    PreparationTime = r.PreparationTime,
                    Images = r.Images,
                    Status = r.Status
                }),
                (int) pageNumber,
                pageSize
            );

            return View(paginatedRecipes);
        }

        [AllowAnonymous]
        // GET: Recipes1/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .FirstOrDefaultAsync(m => m.RecipeID == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // GET: Recipes1/Create
        [HttpGet]
        public IActionResult StepOne()
        {

            ViewData["CategoryID"] = new SelectList(_context.Categories
                .Select(c => new
                {
                    c.CategoryID,
                    CategoryName = c.ApprovedCategoryName ?? (c.PendingCategoryName + (" (pending)"))
                })
                .OrderBy(c => c.CategoryName)
                .ToList(),
                "CategoryID",
                "CategoryName"
                );

            ViewData["CreatedById"] = new SelectList(_context.RecipeUsers, "UserID", "EmailAddress");

            return View(new StepOne());

        }

        //// POST: Recipes1/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StepOne([Bind("RecipeID, RecipeName, CategoryID")] StepOne stepOne)
        {
            if (ModelState.IsValid)
            {
                bool resipeNameExists = RecipeNameExists(stepOne.RecipeName);

                if (resipeNameExists)
                {
                    ModelState.AddModelError("RecipeName", $"The Recipe with the same name already exists");
                    ViewData["CategoryID"] = new SelectList(_context.Categories
                    .Select(c => new
                    {
                        c.CategoryID,
                        CategoryName = c.ApprovedCategoryName ?? (c.PendingCategoryName + (" (pending)"))
                    }), "CategoryID", "CategoryName", stepOne.CategoryID);

                    return View();
                }

                var currentUserEmail = User.Identity?.Name;
                var currentUser = await _context.RecipeUsers
                    .FirstOrDefaultAsync(u => u.EmailAddress == currentUserEmail);

                //if (currentUser == null) 
                // return Unauthorized();

                Recipe recipe = new Recipe
                {
                    RecipeName = stepOne.RecipeName,
                    CategoryID = stepOne.CategoryID,
                    Status = Status.Draft,
                    CreatedById = currentUser.UserID,
                    Created = DateTime.Now,
                };

                _context.Recipes.Add(recipe);
                await _context.SaveChangesAsync();

                return RedirectToAction("StepTwo", new { recipeId = recipe.RecipeID });
            }

            ViewData["CategoryID"] = new SelectList(_context.Categories
                    .Select(c => new
                    {
                        c.CategoryID,
                        CategoryName = c.ApprovedCategoryName ?? (c.PendingCategoryName + (" (pending)"))
                    }), "CategoryID", "CategoryName", stepOne.CategoryID);

            return View(stepOne);
        }

        [HttpGet]
        public async Task<IActionResult> StepTwo(int? recipeId)
        {
            if (recipeId == null)
            {
                return NotFound();
            }

            var recipe = _context?.Recipes.FirstOrDefault(r => r.RecipeID == recipeId);

            if (recipe == null)
            {
                return NotFound();
            }

            var stepTwo = new StepTwo
            {
                RecipeID = recipe.RecipeID,
                RecipeName = recipe.RecipeName,
                CategoryName = recipe.Category?.ApprovedCategoryName
                           ?? (recipe.Category?.PendingCategoryName + (" pending")),

                Ingredients = recipe.RecipeIngredientDetails
                    .Select(ri => new IngredientSelectionViewModel
                    {
                        IngredientID = ri.IngredientID,
                        IngredientName = ri.Ingredient?.ApprovedIngredientName
                                        ?? (ri.Ingredient?.PendingIngredientName + (" (pending)")),
                        Amount = ri.IngredientAmount,
                        MeasurementType = ri.MeasurementType
                    })
                    .ToList()
            };

            ViewData["IngredientList"] = new SelectList(_context.Ingredients
                .Select(i => new
                {
                    i.IngredientID,
                    IngredientName = i.ApprovedIngredientName
                                  ?? (i.PendingIngredientName + (" (pending)"))
                }), "IngredientID", "IngredientName", stepTwo.SelectedIngredientID
            );

            ViewData["Ingredients"] = recipe.RecipeIngredientDetails
                .Select(ri => new IngredientSelectionViewModel
                {
                    IngredientID = ri.IngredientID,
                    IngredientName = ri.Ingredient?.ApprovedIngredientName
                                  ?? (ri.Ingredient?.PendingIngredientName + (" (pending)")),
                    Amount = ri.IngredientAmount,
                    MeasurementType = ri.MeasurementType
                })
                .ToList();
            return View("StepTwo", stepTwo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StepTwo(StepTwo stepTwo, string action)
        {
            if (ModelState.IsValid)
            {

                var recipe = await _context.Recipes
                    .FirstOrDefaultAsync(r => r.RecipeID == stepTwo.RecipeID);

                if (recipe == null)
                {
                    return NotFound();
                }

                if (recipe.RecipeIngredientDetails == null)
                {
                    recipe.RecipeIngredientDetails = new List<RecipeIngredientDetail>();
                }

                var existing = recipe.RecipeIngredientDetails
                    .FirstOrDefault(ri => ri.IngredientID == stepTwo.SelectedIngredientID);

                if (existing == null)
                {
                    recipe.RecipeIngredientDetails.Add(new RecipeIngredientDetail
                    {
                        RecipeID = stepTwo.RecipeID,
                        IngredientID = stepTwo.SelectedIngredientID,
                        IngredientAmount = stepTwo.NewAmount,
                        MeasurementType = stepTwo.NewMeasurementType
                    });
                }
                else
                {
                    existing.IngredientAmount = stepTwo.NewAmount;
                    existing.MeasurementType = stepTwo.NewMeasurementType;
                }
                await _context.SaveChangesAsync();

                if (action == "add")
                    return RedirectToAction("StepTwo", new { recipeId = stepTwo.RecipeID });

                return RedirectToAction("StepThree", new { recipeId = stepTwo.RecipeID });
            }

            // Re-build my step two view if modelstate = false
            var recipeForView = await _context.Recipes
              .FirstOrDefaultAsync(r => r.RecipeID == stepTwo.RecipeID);

            if (recipeForView == null)
            {
                return NotFound();
            }

            ViewData["IngredientList"] = new SelectList(_context.Ingredients
                .Select(i => new
                {
                    i.IngredientID,
                    IngredientName = i.ApprovedIngredientName
                                   ?? (i.PendingIngredientName + (" (pending)"))
                }), "IngredientID", "IngredientName", stepTwo.SelectedIngredientID
                 );

            ViewData["Ingredients"] = recipeForView.RecipeIngredientDetails
                .Select(ri => new IngredientSelectionViewModel
                {
                    IngredientID = ri.IngredientID,
                    IngredientName = ri.Ingredient?.ApprovedIngredientName
                                  ?? (ri.Ingredient?.PendingIngredientName + (" (pending)")),
                    Amount = ri.IngredientAmount,
                    MeasurementType = ri.MeasurementType
                })
                .ToList();

            return View("StepTwo", stepTwo);

        }

        [HttpGet]
        public async Task<IActionResult> StepThree(int? recipeId)
        {
            var recipe = _context?.Recipes.FirstOrDefault(r => r.RecipeID == recipeId);

            if (recipe == null)
            {
                return NotFound();
            }

            var model = new StepThree
            {
                RecipeID = recipe.RecipeID,
                RecipeName = recipe.RecipeName,
                CategoryName = recipe.Category?.ApprovedCategoryName
                           ?? (recipe.Category?.PendingCategoryName + (" (pending)")),

                Ingredients = recipe.RecipeIngredientDetails
                    .Select(ri => new IngredientSelectionViewModel
                    {
                        IngredientID = ri.IngredientID,
                        IngredientName = ri.Ingredient?.ApprovedIngredientName
                                         ?? (ri.Ingredient?.PendingIngredientName + (" (pending)")),
                        Amount = ri.IngredientAmount,
                        MeasurementType = ri.MeasurementType
                    })
                    .ToList()
            };

            return View("StepThree", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StepThree(StepThree model)
        {
            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(r => r.RecipeID == model.RecipeID);

            if (recipe == null)
            {
                return NotFound();
            }

            recipe.Instruction = model.Instruction;
            recipe.Tips = model.Tips;
            recipe.PreparationTime = model.PreparationTime;
            recipe.CookingTime = model.CookingTime;
            recipe.CookingTemperature = model.CookingTemperature;
            recipe.NumberOfServings = model.NumberOfServings;
            recipe.Status = Status.Draft;
            recipe.Images = model.Images;
            recipe.SpecialEquipment = model.SpecialEquipment;
            recipe.YoutubeLinks = model.YoutubeLinks;
            recipe.NutritionalInfo = model.NutritionalInfo;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Congratulation! {recipe.RecipeName} Draft has been created successfully!";

            return RedirectToAction("MyRecipes");
        }


        // GET: Recipes1/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "PendingCategoryName", recipe.CategoryID);
            return View(recipe);
        }

        // POST: Recipes1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("RecipeID,RecipeName,Instruction,Tips,PreparationTime,CookingTime,CookingTemperature,NumberOfServings,CategoryID,Status,UserID,Created")] Recipe recipe)
        {
            if (id != recipe.RecipeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.RecipeID))
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
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "ApprovedCategoryName" ?? "PendingCategoryName", recipe.CategoryID);
            return View(recipe);
        }

        // GET: Recipes1/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .FirstOrDefaultAsync(m => m.RecipeID == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Recipes1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Share(int id)
        {
            var model = new ShareRecipeViewModel
            {
                RecipeID = id
            };

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Share(ShareRecipeViewModel shareVM)
        {
            if (ModelState.IsValid)
            {
                string subject;
                string content;
                var recipeUrl = Url.Action("Details", "Recipes1", new { id = shareVM.RecipeID }, Request.Scheme);

                var recipe = await _context.Recipes
                    .Where(r => r.RecipeID == shareVM.RecipeID)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrWhiteSpace(shareVM.Subject))
                {
                    subject = "Someone Wants to Share a Recipe With You!";
                }
                else
                {
                    subject = shareVM.Subject;
                }

                if (string.IsNullOrWhiteSpace(shareVM.Message))
                {
                    content = $"Hi! A friend thought you might enjoy this recipe.\n\nRecipe: {recipe.RecipeName}\nLink: {recipeUrl}";
                }
                else
                {
                    content = $"{shareVM.Message}\n\nRecipe: {recipe.RecipeName}\nLink: {recipeUrl}";
                }

                var message = new EmailMessage(
                    new string[] { shareVM.RecipientEmail },
                subject,
                content
                );

                _emailSender.SendEmail(message);

                TempData["SuccessMessage"] = $"Thank you for sharing our recipe! You successfully shared {recipe.RecipeName} with {shareVM.RecipientEmail}";

                return RedirectToAction(nameof(Index));
            }

            return View(shareVM);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Report(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            return View(new Report
            {
                RecipeID = id.Value
            });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Report(Report report)
        {
            if (ModelState.IsValid)
            {
                var recipe = await _context.Recipes
                    .Where(r => r.RecipeID == report.RecipeID)
                    .Select(r => new { r.RecipeName })
                    .FirstOrDefaultAsync();

                var reportMessage = new EmailMessage(
                    new string[] { "admin@cookingislife.com" },
                    $"New Report Submitted",
$@"A new report has been submitted on the website.

-------------------------
Report Information
-------------------------
Report ID: {report.ReportID}
Recipe ID: {report.RecipeID}
Recipe Name: {recipe.RecipeName}

-------------------------
Reporter Details
-------------------------
Full Name: {report.FullName ?? "N/A"}
Email: {report.Email}

-------------------------
Report Reason
-------------------------
Reason: {report.Reason}

-------------------------
Submitted At
-------------------------
{DateTime.Now}
"
);

                _emailSender.SendEmail(reportMessage);

                _context.Reports.Add(report);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thank you — your report has been submitted. You will be notified by email as soon as we review the recipe!";

                return RedirectToAction("Details", new { id = report.RecipeID });
            }

            return View(report);
        }

        [NonAction]
        private bool RecipeExists(int? id)
        {
            return _context.Recipes.Any(e => e.RecipeID == id);
        }

        // Added this code to handle Recipe Name uniqness check 
        [NonAction]
        private bool RecipeNameExists(string? recipeName)
        {
            return _context.Recipes.Any(r => r.RecipeName == recipeName);
        }
    }
}
