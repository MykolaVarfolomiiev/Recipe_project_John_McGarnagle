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
    public class Categories1Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public Categories1Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: Categories1
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Categories.Include(c => c.CreatedBy);
            return View(await 
                applicationDbContext
                .OrderBy(c => c.ApprovedCategoryName)
                .ThenBy(c => c.PendingCategoryName)
                .ToListAsync()
            );
        }

        [AllowAnonymous]
        // GET: Categories1/Details/5
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

        // GET: Categories1/Create
        public IActionResult Create()
        {
            ViewData["CreatedById"] = new SelectList(_context.RecipeUsers, "UserID", "EmailAddress");
            return View();
        }

        // POST: Categories1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryID,ApprovedCategoryName,ApprovedCategoryDescription,PendingCategoryName,PendingCategoryDescription,CreatedById")] Category category)
        {
            if (CategoryExists(category.PendingCategoryName) || CategoryExists(category.ApprovedCategoryName))
            {
                ModelState.Remove("PendingCategoryName");
                ModelState.AddModelError("PendingCategoryName", $"The category \"{category.PendingCategoryName}\" already exists");
            }

            if (ModelState.IsValid)
            {
                Category newCategory = new Category
                {
                    PendingCategoryName = category.PendingCategoryName,
                    PendingCategoryDescription = category.PendingCategoryDescription
                };

                _context.Add(newCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatedById"] = new SelectList(_context.RecipeUsers, "UserID", "EmailAddress", category.CreatedById);
            return View(category);
        }

        // GET: Categories1/Edit/5
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
            ViewData["CreatedById"] = new SelectList(_context.RecipeUsers, "UserID", "EmailAddress", category.CreatedById);
            return View(category);
        }

        // POST: Categories1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("CategoryID,ApprovedCategoryName,ApprovedCategoryDescription,PendingCategoryName,PendingCategoryDescription,CreatedById")] Category category)
        {
            if (id != category.CategoryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryID))
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
            ViewData["CreatedById"] = new SelectList(_context.RecipeUsers, "UserID", "EmailAddress", category.CreatedById);
            return View(category);
        }

        // GET: Categories1/Delete/5
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

        // POST: Categories1/Delete/5
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
            return _context.Categories.Any(e => e.PendingCategoryName == name);
        }
    }
}
