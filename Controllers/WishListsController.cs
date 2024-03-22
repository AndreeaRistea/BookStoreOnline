using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect_CE.Data;
using Proiect_CE.Models;
using System.Security.Claims;

namespace Proiect_CE.Controllers
{
    public class WishListsController : Controller
    {
        private readonly BookStoreContext _context;

        public WishListsController(BookStoreContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> IndexAdmin()
        {
            return _context.WishLists != null ?
                         View(await _context.WishLists.ToListAsync()) :
                         Problem("Entity set 'BookStoreContext.WishLists'  is null.");
        }

        // GET: WishLists
        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var user = await _context.Users.FirstOrDefaultAsync(b => b.InnerUserId == userId);

            var wishList = await _context.WishLists
                .Include(w => w.books)
                .FirstOrDefaultAsync(w => w.UserId == user.id);

            if (wishList != null)
            {
                return View(wishList);
            }
            else
            {
                return Problem("Entity set 'BookStoreContext.WishLists' is null.");
            }
        }

        // GET: WishLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.WishLists == null)
            {
                return NotFound();
            }

            var wishList = await _context.WishLists
                .FirstOrDefaultAsync(m => m.id == id);
            if (wishList == null)
            {
                return NotFound();
            }

            return View(wishList);
        }

        // GET: WishLists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WishLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,UserId")] WishList wishList)
        {
            if (ModelState.IsValid)
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                wishList.User = await _context.Users.FirstOrDefaultAsync(b => b.InnerUserId == userId);
                _context.Add(wishList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(wishList);
        }

        // GET: WishLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.WishLists == null)
            {
                return NotFound();
            }

            var wishList = await _context.WishLists.FindAsync(id);
            if (wishList == null)
            {
                return NotFound();
            }
            return View(wishList);
        }

        // POST: WishLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,UserId")] WishList wishList)
        {
            if (id != wishList.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wishList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WishListExists(wishList.id))
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
            return View(wishList);
        }

        // GET: WishLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.WishLists == null)
            {
                return NotFound();
            }

            var wishList = await _context.WishLists
                .FirstOrDefaultAsync(m => m.id == id);
            if (wishList == null)
            {
                return NotFound();
            }

            return View(wishList);
        }

        // POST: WishLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.WishLists == null)
            {
                return Problem("Entity set 'BookStoreContext.WishLists' is null.");
            }

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var wishList =
                await _context.WishLists
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.id == id);

            if (wishList != null && wishList.User.InnerUserId == userId)
            {
                _context.WishLists.Remove(wishList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }

        private bool WishListExists(int id)
        {
            return (_context.WishLists?.Any(e => e.id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> RemoveFromWishlist(int wishListId, string bookId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var wishList = await _context.WishLists
                .Include(w => w.books)
                .FirstOrDefaultAsync(w => w.id == wishListId && w.User.InnerUserId == userId);

            if (wishList != null)
            {
                var book = wishList.books.FirstOrDefault(b => b.ISBN == bookId);

                if (book != null)
                {
                    wishList.books.Remove(book);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
