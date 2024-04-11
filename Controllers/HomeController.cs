using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect_CE.Data;
using Proiect_CE.Models;
using System.Diagnostics;

namespace Proiect_CE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookStoreContext _context;
        public HomeController(ILogger<HomeController> logger, BookStoreContext context)
        {
            _logger = logger;
            _context = context;
        }

        //public async Task<IActionResult> Index()
        //{
        //    //return View();

        //    return _context.Genres != null ?
        //                View(await _context.Genres.ToListAsync()) :
        //                Problem("Entity set 'BookStoreContext.Genres'  is null.");
        //}

        //public async Task<IActionResult> Index()
        //{
        //    var books = await _context.Books
        //        .Include(b => b.Authors)
        //        .Include(b => b.Genre)
        //        .Include(b => b.PublishingHouse)
        //        .GroupBy(g => g.Genre)
        //        .ToListAsync();

        //    return View(books);
        //}

        public async Task<IActionResult> Index(string searchString)
        {
            var  books = from b in _context.Books.Include(b => b.Authors).Include(b => b.Genre)
                         .Include(b => b.PublishingHouse)
                         select b;

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                books = books.Where(b => b.Title.ToLower().Contains(searchString));

                return View(await books
                  .GroupBy(g => g.Genre)
                  .ToListAsync()); 
            }

            return View(await books
                .GroupBy(g => g.Genre)
                .ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}