using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect.Models;
using Proiect_CE.Data;

namespace Proiect.Controllers
{
    public class SpecificationsController : Controller
    {
        private readonly BookStoreContext _context;

        public SpecificationsController(BookStoreContext context)
        {
            _context = context;
        }
        // GET: SpecificationsController
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSpecification(string bookId, [Bind("Name, Value")] Specification specification)
        {
            if (bookId == null)
            {
                return BadRequest("Book ID is required.");
            }

            var book = await _context.Books.Include(b => b.Specifications).FirstOrDefaultAsync(b => b.ISBN == bookId);
            if (book == null)
            {
                return NotFound();
            }

            specification.BookId = book.ISBN;
            book.Specifications.Add(specification);

            _context.Specifications.Add(specification);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = bookId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditSpecification(int id, [Bind("Name, Value")] Specification specification)
        {
            var spec = await _context.Specifications.FindAsync(id);
            if (spec == null)
            {
                return NotFound();
            }

            spec.Name = specification.Name;
            spec.Value = specification.Value;

            _context.Update(spec);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = spec.BookId });
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSpecification(int id)
        {
            var spec = await _context.Specifications.FindAsync(id);
            if (spec == null)
            {
                return NotFound();
            }

            _context.Specifications.Remove(spec);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = spec.BookId });
        }


    }
}
