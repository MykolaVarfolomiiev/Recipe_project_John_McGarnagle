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

namespace mvc2025TermProject.Controllers
{
    [Authorize]
    public class Ingredients1Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public Ingredients1Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: Ingredients1
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ingredients.Include(i => i.CreatedBy);
            return View(await
                applicationDbContext
                .OrderBy(i => i.ApprovedIngredientName)
                .ThenBy(i => i.PendingIngredientName)
                .ToListAsync()
            );
        }

        [AllowAnonymous]
        // GET: Ingredients1/Details/5
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

        // GET: Ingredients1/Create
        public IActionResult Create()
        {
            ViewData["CreatedById"] = new SelectList(_context.RecipeUsers, "UserID", "EmailAddress");
            return View();
        }

        // POST: Ingredients1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IngredientID,ApprovedIngredientName,ApprovedIngredientType,ApprovedIngredientDetails,PendingIngredientName,PendingIngredientType,PendingIngredientDetails,CreatedById")] Ingredient ingredient)
        {
            if (IngredientExists(ingredient.PendingIngredientName) || IngredientExists(ingredient.ApprovedIngredientName))
            {
                ModelState.Remove("PendingIngredientName");
                ModelState.AddModelError("Name", $"The ingredient \"{ingredient.PendingIngredientName}\" already exists");
            }

            if (ModelState.IsValid)
            {
                Ingredient newIngredient = new Ingredient
                {
                    PendingIngredientName = ingredient.PendingIngredientName,
                    PendingIngredientType = ingredient.PendingIngredientType,
                    PendingIngredientDetails = ingredient.PendingIngredientDetails
                };

                _context.Add(newIngredient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedById"] = new SelectList(_context.RecipeUsers, "UserID", "EmailAddress", ingredient.CreatedById);
            return View(ingredient);
        }

        // GET: Ingredients1/Edit/5
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
            ViewData["CreatedById"] = new SelectList(_context.RecipeUsers, "UserID", "EmailAddress", ingredient.CreatedById);
            return View(ingredient);
        }

        // POST: Ingredients1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("IngredientID,ApprovedIngredientName,ApprovedIngredientType,ApprovedIngredientDetails,PendingIngredientName,PendingIngredientType,PendingIngredientDetails,CreatedById")] Ingredient ingredient)
        {
            if (id != ingredient.IngredientID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ingredient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngredientExists(ingredient.IngredientID))
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
            ViewData["CreatedById"] = new SelectList(_context.RecipeUsers, "UserID", "EmailAddress", ingredient.CreatedById);
            return View(ingredient);
        }

        // GET: Ingredients1/Delete/5
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

        // POST: Ingredients1/Delete/5
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
            return _context.Ingredients.Any(e => e.PendingIngredientName == name);
        }
    }
}
