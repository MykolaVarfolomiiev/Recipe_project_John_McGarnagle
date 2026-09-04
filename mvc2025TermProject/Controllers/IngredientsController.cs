using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Data;
using mvc2025TermProject.Models;
using mvc2025TermProject.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc2025TermProject.Controllers
{
    [Authorize]
    public class IngredientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IngredientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: Ingredients
        public async Task<IActionResult> Index()
        {
            var ingredients = await _context.Ingredients
                .Include(i => i.CreatedBy)
                .ToListAsync();

            var orderedIngredients = ingredients
                .OrderBy(i => (i.ApprovedIngredientName ?? i.PendingIngredientName).Trim().ToUpper())
                .ToList();

            return View(orderedIngredients);
        }

        [AllowAnonymous]
        // GET: Ingredients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .Include(i => i.CreatedBy)
                .FirstOrDefaultAsync(m => m.IngredientID == id);

            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        // GET: Ingredients/Create
        public IActionResult Create()
        {
            ViewData["CreatedById"] = new SelectList(_context.RecipeUsers, "UserID", "EmailAddress");
            return View();
        }

        // POST: Ingredients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Type,Details,CreatedById")] IngredientsCreateViewModel ingredientVM)
        {
            if (IngredientExists(ingredientVM.Name))
            {
                ModelState.Remove("PendingIngredientName");
                ModelState.AddModelError("PendingIngredientName", $"The ingredient \"{ingredientVM.Name}\" already exists");
            }

            if (ModelState.IsValid)
            {
                var currentUserEmail = User.Identity?.Name;
                var currentUser = await _context.RecipeUsers
                    .FirstOrDefaultAsync(u => u.EmailAddress == currentUserEmail);

                Ingredient newIngredient = new Ingredient
                {
                    PendingIngredientName = ingredientVM.Name,
                    PendingIngredientType = ingredientVM.Type,
                    PendingIngredientDetails = ingredientVM.Details,
                    CreatedById = currentUser.UserID
                };

                _context.Add(newIngredient);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"{ingredientVM.Name} has been created and a decision will be made as soon as possible. " +
                                             $"You will be notified as soon as a decision has been made.";
                return RedirectToAction(nameof(Index));
            }

            return View(ingredientVM);
        }

        // GET: Ingredients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients.FindAsync(id);

            if (ingredient == null)
            {
                return NotFound();
            }

            var ingredientVM = new IngredientsEditViewModel
            {
                IngredientID = ingredient.IngredientID,
                Name = ingredient.PendingIngredientName,
                ApprovedName = ingredient.ApprovedIngredientName,
                Type = ingredient.PendingIngredientType,
                ApprovedType = ingredient.ApprovedIngredientType,
                Details = ingredient.PendingIngredientDetails,
                ApprovedDetails = ingredient.ApprovedIngredientDetails,
                CreatedById = ingredient.CreatedById,
            };

            ViewData["CreatedById"] = new SelectList(_context.RecipeUsers, "UserID", "EmailAddress", ingredient.CreatedById);
            return View(ingredientVM);
        }

        // POST: Ingredients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("IngredientID,,Name,Type,Details,CreatedById")] IngredientsEditViewModel ingredientVM)
        {
            if (id != ingredientVM.IngredientID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var ingredient = await _context.Ingredients.FindAsync(id);

                    ingredient.PendingIngredientName = ingredientVM.Name?.Trim();
                    ingredient.PendingIngredientType = ingredientVM.Type?.Trim();
                    ingredient.PendingIngredientDetails = ingredientVM.Details?.Trim();

                    _context.Update(ingredient);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Your proposed update for {ingredientVM.Name} has been sent and a decision will be made as soon as possible. " +
                                                 $"You will be notified as soon as a decision has been made.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngredientExists(ingredientVM.IngredientID))
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
            ViewData["CreatedById"] = new SelectList(_context.RecipeUsers, "UserID", "EmailAddress", ingredientVM.CreatedById);
            return View(ingredientVM);
        }

        // GET: Ingredients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .Include(i => i.CreatedBy)
                .FirstOrDefaultAsync(m => m.IngredientID == id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        // POST: Ingredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient != null)
            {
                _context.Ingredients.Remove(ingredient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        private bool IngredientExists(int? id)
        {
            return _context.Ingredients.Any(e => e.IngredientID == id);
        }

        [NonAction]
        private bool IngredientExists(string name)
        {
            return _context.Ingredients.Any(e => 
            e.PendingIngredientName == name ||
            e.ApprovedIngredientName == name
            );
        }
    }
}

