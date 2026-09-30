using Microsoft.AspNetCore.Mvc;
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
            return View();
        }

        // POST: /ServiceRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ServiceRequest request)
        {
            if (!ModelState.IsValid) // enforces mandatory field acceptance criterion
            {
                return View(request);
            }

            _context.ServiceRequests.Add(request);
            _context.SaveChanges(); // unique ID generated automatically by the database

            return RedirectToAction(nameof(Details), new { id = request.Id });
        }

        // GET: /ServiceRequests/Details/5
        public IActionResult Details(int id)
        {
            var request = _context.ServiceRequests.Find(id);
            if (request == null) return NotFound();
            return View(request);
        }
    }
}

