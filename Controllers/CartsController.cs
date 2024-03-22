using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect.Models;
using Proiect_CE.Data;
using System.Security.Claims;

namespace Proiect.Controllers
{
    public class CartsController : Controller
    {
        private readonly BookStoreContext _context;

        public CartsController(BookStoreContext context)
        {
            _context = context;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FirstOrDefaultAsync(b => b.InnerUserId == userId);
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .ThenInclude(b => b.Authors)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .ThenInclude(b => b.PublishingHouse)
                .FirstOrDefaultAsync(c => c.UserId == user.id);

            List<Cart> allCarts = null;
            var isAdmin = User.IsInRole("Admin");
            if (isAdmin)
            {
                allCarts = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .ThenInclude(b => b.Authors)
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .ThenInclude(b => b.PublishingHouse)
                .ToListAsync();
            }

            if (cart != null || isAdmin)
            {
                return View(Tuple.Create<Cart, IEnumerable<Cart>>(cart, allCarts));
            }
            else
            {
                return Problem("Cart not found for the current user.");
            }
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .FirstOrDefaultAsync(m => m.CartId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartId,UserId")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CartId,UserId")] Cart cart)
        {
            if (id != cart.CartId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.CartId))
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
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .FirstOrDefaultAsync(m => m.CartId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Carts == null)
            {
                return Problem("Entity set 'BookStoreContext.Carts'  is null.");
            }
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return (_context.Carts?.Any(e => e.CartId == id)).GetValueOrDefault();
        }

        [HttpPost]
        [Route("AddToCart")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddToCart(string bookId, int quantity)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FirstOrDefaultAsync(b => b.InnerUserId == userId);

            var book = await _context.Books.FindAsync(bookId);

            if (user == null || book == null)
            {
                return Problem("User or book not found.");
            }

            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == user.id);

            if (cart == null)
            {
                cart = new Cart { UserId = user.id };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.ISBN == book.ISBN);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
            }
            else
            {
                var newCartItem = new CartItem
                {
                    Quantity = quantity,
                    ISBN = book.ISBN,
                    Book = book,
                    CartId = cart.CartId
                };

                _context.CartItems.Add(newCartItem);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("Carts/UpdateQuantity")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int change)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);

            if (cartItem != null)
            {
                if (cartItem.Quantity + change >= 1)
                {
                    cartItem.Quantity += change;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return BadRequest("Nu se poate efectua operația. Cantitatea nu poate fi mai mică decât 1.");
                }
            }

            return Ok();
        }
        public async Task<IActionResult> RemoveFromCart(int cartId, int cartItemId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var cart = await _context.Carts
            .Include(w => w.CartItems)
            .ThenInclude(b => b.Book)
                .FirstOrDefaultAsync(w => w.CartId == cartId && w.User.InnerUserId == userId);

            if (cart != null)
            {
                var cartItem = cart.CartItems.FirstOrDefault(b => b.CartItemId == cartItemId);

                if (cartItem != null)
                {
                    cart.CartItems.Remove(cartItem);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult PlaceOrder()
        {
            return View("Checkout");
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FirstOrDefaultAsync(b => b.InnerUserId == userId);

            var cart = await _context.Carts
            .Include(w => w.CartItems)
            .ThenInclude(b => b.Book)
                .FirstOrDefaultAsync(w => w.User.InnerUserId == userId);

            if (cart != null && cart.CartItems.Any())
            {
                var cartItem = cart.CartItems.FirstOrDefault();

                var order = new Order
                {
                    OrderDate = DateTime.Now.ToUniversalTime(),
                    UserId = user.id,
                    OrderItems = cart.CartItems.Select(ci => new OrderItem
                    {
                        Quantity = ci.Quantity,
                        ISBN = ci.Book.ISBN,
                        Book = ci.Book
                    }).ToList()
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();

                TempData["OrderConfirmationMessage"] = "Comanda a fost plasată cu succes!";
                return View("OrderConfirmation");
            }
            else
            {
                return Problem(nameof(Checkout));
            }
        }

    }
}

