using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CivicConnect.Data;
using CivicConnect.Models;

namespace CivicConnect.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly AppDbContext _context;

        public ServiceRequestsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /ServiceRequests/Create
        public IActionResult Create()
        {
            LoadCategories();
            return View();
        }

        // POST: /ServiceRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ServiceRequest request)
        {
            // The dropdown can be bypassed with a crafted POST, so confirm the category
            // exists and is still active before saving (FR-002).
            if (!_context.Categories.Any(c => c.Id == request.CategoryId && c.IsActive))
            {
                ModelState.AddModelError(nameof(ServiceRequest.CategoryId), "Please select a valid category.");
            }

            if (!ModelState.IsValid) // enforces mandatory field acceptance criterion
            {
                LoadCategories(request.CategoryId);
                return View(request);
            }

            _context.ServiceRequests.Add(request);
            _context.SaveChanges(); // unique ID generated automatically by the database

            return RedirectToAction(nameof(Details), new { id = request.Id });
        }

        // GET: /ServiceRequests/Details/5
        public IActionResult Details(int id)
        {
            var request = _context.ServiceRequests
                .Include(r => r.Category)
                .FirstOrDefault(r => r.Id == id);
            if (request == null) return NotFound();
            return View(request);
        }

        // Only active categories are offered to requesters.
        private void LoadCategories(int? selectedId = null)
        {
            var categories = _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToList();
            ViewBag.Categories = new SelectList(categories, nameof(Category.Id), nameof(Category.Name), selectedId);
        }
    }
}

