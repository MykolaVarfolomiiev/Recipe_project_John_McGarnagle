using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Data;
using mvc2025TermProject.Models;
using mvc2025TermProject.Models.Enums;
using mvc2025TermProject.Models.ViewModels;

namespace mvc2025TermProject.Controllers
{
    [Authorize]
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: Recipes
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.Recipes.Include(r => r.Category).Include(r => r.Owner);
            //return View(await applicationDbContext.ToListAsync());

            IEnumerable<RecipeViewModel> recipeList =
                _context.Recipes.Select(r =>

                new RecipeViewModel
                {
                    RecipeID = r.RecipeID,
                    RecipeName = r.RecipeName,
                    //CreatedBy = r.CreatedBy,
                    //Status = r.Status,
                    //Instruction = r.Instruction,
                    //PreparationTime = r.PreparationTime,
                    //CookingTime = r.CookingTime,
                    //NumberOfServings = r.NumberOfServings,
                    Category = r.Category,
                    //RecipeIngredientDetails = r.RecipeIngredientDetails

                });
            return View(recipeList);
        }
        [AllowAnonymous]
        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.CreatedBy)
                .FirstOrDefaultAsync(m => m.RecipeID == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // GET: Recipes/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "Name");
            ViewData["OwnerID"] = new SelectList(_context.RecipeUsers, "UserID", "FirstName");

            // Added this
            ViewBag.Status = new SelectList(Enum.GetValues(typeof(Status))); 
            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecipeID,RecipeName,OwnerID,Status,Instruction,PreparationTime,CookingTime,NumberOfServings,CategoryID")] RecipeCreateViewModel recipeCreateVM)
        {
            //if (ModelState.IsValid)
            //{
            //    _context.Add(recipe);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "Name", recipe.CategoryID);
            //ViewData["OwnerID"] = new SelectList(_context.Owners, "OwnerID", "FirstName", recipe.OwnerID);
            //return View(recipe);
            
            if (ModelState.IsValid)
            {
                Recipe newRecipe = new Recipe
                {
                    RecipeName = recipeCreateVM.RecipeName,
                    CreatedBy = recipeCreateVM.CreatedBy,
                    //Status = Status.Draft,
                    Status = recipeCreateVM.Status,
                    Instruction = recipeCreateVM.Instruction,
                    PreparationTime = recipeCreateVM.PreparationTime,
                    CookingTime = recipeCreateVM.CookingTime,
                    NumberOfServings = recipeCreateVM.NumberOfServings,
                    CategoryID = recipeCreateVM.CategoryID,
                    Category = recipeCreateVM.Category,
                    Images = recipeCreateVM.Images
                };
                _context.Add(newRecipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(recipeCreateVM);
        }

        // GET: Recipes/Edit/5
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
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "Name", recipe.CategoryID);
            ViewData["CreatedById"] = new SelectList(_context.RecipeUsers, "UserID", "EmailAddress");
            // Added this
            ViewBag.Status = new SelectList(Enum.GetValues(typeof(Status)));
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("RecipeID,RecipeName,UserID,Status,Instruction,PreparationTime,CookingTime,NumberOfServings,CategoryID")] Recipe recipe)
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
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "Name", recipe.CategoryID);
            ViewData["CreatedById"] = new SelectList(_context.RecipeUsers, "UserID", "EmailAddress");
            return View(recipe);
        }

        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.CreatedBy)
                .FirstOrDefaultAsync(m => m.RecipeID == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Recipes/Delete/5
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

        private bool RecipeExists(int? id)
        {
            return _context.Recipes.Any(e => e.RecipeID == id);
        }

        // Added this code to handle Recipe Name uniqness check 
        [NonAction]
        private bool RecipeExists(string? name)
        {
            return _context.Recipes.Any(r => r.RecipeName.ToLower() == name.ToLower());
        }
    }
}
