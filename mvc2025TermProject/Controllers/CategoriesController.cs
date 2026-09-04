using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Data;
using mvc2025TermProject.Models;
using mvc2025TermProject.Models.ViewModels;

namespace mvc2025TermProject.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [AllowAnonymous]
        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.CreatedBy)
                .ToListAsync();

            var orderedCategories = categories
                .OrderBy(c => (c.ApprovedCategoryName ?? c.PendingCategoryName).Trim().ToUpper())
                .ToList();

            return View(orderedCategories);
        }

        [AllowAnonymous]
        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.CreatedBy)
                .FirstOrDefaultAsync(m => m.CategoryID == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            ViewData["CreatedById"] = new SelectList(_context.RecipeUsers, "UserID", "EmailAddress");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,CreatedById,CategoryID")] CategoryCreateViewModel categoryVM)
        {
            if (CategoryExists(categoryVM.Name)) {
                ModelState.Remove("Name");
                ModelState.AddModelError("Name", $"The category \"{categoryVM.Name}\" already exists");
            }

            if (ModelState.IsValid)
            { 
                var currentUserEmail = User.Identity?.Name;
                var currentUser = await _context.RecipeUsers
                    .FirstOrDefaultAsync(u => u.EmailAddress == currentUserEmail);

                Category newCategory = new Category
                {
                    PendingCategoryName = categoryVM.Name,
                    PendingCategoryDescription = categoryVM.Description,
                    CreatedById = currentUser.UserID
                };

                _context.Add(newCategory);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"{categoryVM.Name} has been created and a decision will be made as soon as possible. " +
                                             $"You will be notified as soon as a decision has been made.";
                return RedirectToAction(nameof(Index));
            }

            return View(categoryVM);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var categoryVM = new CategoryEditViewModel
            {
                CategoryID = category.CategoryID,
                Name = category.PendingCategoryName,
                Description = category.PendingCategoryDescription,
                CreatedById = category.CreatedById,
                ApprovedName = category.ApprovedCategoryName,
                ApprovedDescription = category.ApprovedCategoryDescription
            };

            ViewData["CreatedById"] = new SelectList(_context.RecipeUsers, "UserID", "EmailAddress", category.CreatedById);
            return View(categoryVM);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("CategoryID,Name,Description,CreatedById")] CategoryEditViewModel categoryVM)
        {
            if (id != categoryVM.CategoryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var category = await _context.Categories.FindAsync(id);

                    category.PendingCategoryName = categoryVM.Name?.Trim();
                    category.PendingCategoryDescription = categoryVM.Description?.Trim();
                    category.CreatedById = categoryVM.CreatedById;

                    _context.Update(category);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Your proposed update for {categoryVM.Name} has been sent and a decision will be made as soon as possible. " +
                             $"You will be notified as soon as a decision has been made.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(categoryVM.CategoryID))
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
            ViewData["CreatedById"] = new SelectList(_context.RecipeUsers, "UserID", "EmailAddress", categoryVM.CreatedById);
            return View(categoryVM);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.CreatedBy)
                .FirstOrDefaultAsync(m => m.CategoryID == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        private bool CategoryExists(int? id)
        {
            return _context.Categories.Any(e => e.CategoryID == id);
        }

        [NonAction]
        private bool CategoryExists(string name)
        {
            return _context.Categories.Any(c =>
                c.PendingCategoryName == name ||
                c.ApprovedCategoryName == name
            );
        }
    }
}
