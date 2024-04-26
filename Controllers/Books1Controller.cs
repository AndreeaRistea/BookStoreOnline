using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proiect.Search;
using Proiect_CE.Data;
using Proiect_CE.Models;
using System;
using System.Data;
using System.Security.Claims;
using static Lucene.Net.Search.SimpleFacetedSearch;

namespace Proiect_CE.Controllers
{
    public class Books1Controller : Controller
    {
        private readonly BookStoreContext _context;
        private readonly BookSearchEngine _bookSearchEngine;
        public Books1Controller(BookStoreContext context, BookSearchEngine bookSearchEngine)
        {
            _context = context;
            _bookSearchEngine = bookSearchEngine;
        }

        // GET: Books1
        [AllowAnonymous]
        public async Task<IActionResult> Index(string title)
        {
            if (title != null)
            {
                var books = _context.Books.Include(b => b.Authors).Include(b => b.Genre)
                .Include(b => b.PublishingHouse).Include(b => b.Specifications).Where(b => b.Title.ToLower().Contains(title.ToLower()));
                return View(await books.ToListAsync());
            }
            else
            {
                var books = _context.Books.Include(b => b.Authors).Include(b => b.Genre).Include(b => b.Specifications)
                    .Include(b => b.PublishingHouse);
                return View(await books.ToListAsync());
            }

        }

        public async Task<IActionResult> SearchByAuthor(string author)
        {
            if (author != null)
            {
                var books = _context.Books.Include(b => b.Authors).Include(b => b.Genre)
                .Include(b => b.PublishingHouse)
                .Include(b => b.Specifications)
                .Where(b => b.Authors.Name.ToLower().Contains(author.ToLower()));
                return View("Index", await books.ToListAsync());
            }
            else
            {
                var books = _context.Books.Include(b => b.Authors).Include(b => b.Genre)
                .Include(b => b.PublishingHouse).Include(b => b.Specifications);
                return View("Index", await books.ToListAsync());
            }

        }
    

        [AllowAnonymous]
        public async Task<IActionResult> BookGroupByGenre(int genreId)
        {
            var books = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Genre)
                .Include(b => b.PublishingHouse)
                .Where(b => b.GenreId == genreId)
                .ToListAsync();

            return View(books);
        }
        [AllowAnonymous]
        public async Task<IActionResult> BookGroupByAuthor(int authorId)
        {
            var books = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Genre)
                .Include(b => b.PublishingHouse)
                .Where(b => b.AuthorId == authorId)
                .ToListAsync();

            return View(books);
        }
        // GET: Books1/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Genre)
                .Include(b => b.PublishingHouse)
                .FirstOrDefaultAsync(m => m.ISBN == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books1/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Name");
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name");
            ViewData["PublishingHouseId"] = new SelectList(_context.PublishingHouses, "Id", "Name");
            return View();
        }

        // POST: Books1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ISBN,Title,Description, PublishingHouseId," +
            "Price,DatePublishing,GenreId,AuthorId,Stock,,Specifications,ImageFile,FragmentFile")] Book book)
        {
            book.DatePublishing = book.DatePublishing.ToUniversalTime();
            if (book.ImageFile != null && book.ImageFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await book.ImageFile.CopyToAsync(ms);
                    book.Image = ms.ToArray();
                }
            }
            if (book.FragmentFile != null && book.FragmentFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await book.FragmentFile.CopyToAsync(ms);
                    book.FileText = ms.ToArray();
                    book.FragmentFileName = book.FragmentFile.FileName;
                }
            }
            if (book.Specifications != null && book.Specifications.Any())
            {
                foreach (var specification in book.Specifications)
                {
                    specification.BookId = book.ISBN; // Asociați specificația cu ISBN-ul cărții
                    _context.Specifications.Add(specification);
                }
            }
            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        [AllowAnonymous]
        public async Task<IActionResult> OpenFragment(string id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null || book.FileText == null)
            {
                return NotFound();
            }
            return File(book.FileText, "application/pdf"); 
        }

        // GET: Books1/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Name", book.AuthorId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", book.GenreId);
            ViewData["PublishingHouseId"] = new SelectList(_context.PublishingHouses, "Id", "Name");
            return View(book);
        }

        // POST: Books1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, [Bind("ISBN,Title,Description, PublishingHouseId, Price, DatePublishing, GenreId, AuthorId, Stock, ImageFile, FragmentFile, Specifications")] Book book)
        {
            if (id != book.ISBN)
            {
                return NotFound();
            }

            book.DatePublishing = book.DatePublishing.ToUniversalTime();

            try
            {
                // Procesați imaginea și fragmentul dacă au fost încărcate
                if (book.ImageFile != null && book.ImageFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await book.ImageFile.CopyToAsync(ms);
                        book.Image = ms.ToArray();
                    }
                }
                if (book.FragmentFile != null && book.FragmentFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await book.FragmentFile.CopyToAsync(ms);
                        book.FileText = ms.ToArray();
                        book.FragmentFileName = book.FragmentFile.FileName;
                    }
                }

                // Procesați specificațiile
                if (book.Specifications != null)
                {
                    // Iterați prin specificațiile din formular
                    foreach (var specification in book.Specifications)
                    {
                        // Dacă specificația are un ID, este o specificație existentă și trebuie actualizată
                        if (specification.Id > 0)
                        {
                            var existingSpec = await _context.Specifications.FindAsync(specification.Id);
                            if (existingSpec != null)
                            {
                                existingSpec.Name = specification.Name;
                                existingSpec.Value = specification.Value;
                            }
                        }
                        else
                        {
                            // Dacă specificația este nouă, asociați-o cu cartea și adăugați-o în contextul bazei de date
                            specification.BookId = book.ISBN;
                            _context.Specifications.Add(specification);
                        }
                    }
                }

                // Actualizați cartea în contextul bazei de date
                _context.Update(book);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.ISBN))
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


        // GET: Books1/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(m => m.ISBN == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'BookStoreContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(string id)
        {
            return (_context.Books?.Any(e => e.ISBN == id)).GetValueOrDefault();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddToWishlist(string bookId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Error");
            }

            var wishlist = await _context.WishLists.FirstOrDefaultAsync(w => w.User.InnerUserId == userId);

            if (wishlist == null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.InnerUserId == userId);
                if (user is null)
                {
                    user = new User { InnerUserId = userId };
                    _context.Users.Add(user);
                }

                wishlist = new WishList { User = user, books = new List<Book>() };
                _context.WishLists.Add(wishlist);
                await _context.SaveChangesAsync();

                wishlist.UserId = wishlist.User.id;
                await _context.SaveChangesAsync();
            }

            var book = await _context.Books.FindAsync(bookId);

            if (book != null)
            {
                wishlist.books.Add(book);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "WishLists");
        }
    }
}
