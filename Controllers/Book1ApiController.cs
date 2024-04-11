
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect_CE.Data;
using Proiect_CE.Models;

namespace Proiect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Book1ApiController : ControllerBase
    {
        private readonly BookStoreContext _context;
        public Book1ApiController(BookStoreContext context)
        {
            _context = context;
        }

        [Produces("application/json")]
        [HttpGet ("search/title")]
        public async Task <IActionResult> Search()
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();

                var title = _context.Books.Where(b => b.Title.ToLower()
                            .StartsWith(term.ToLower())).Select(b => b.Title).Distinct().ToList();
                return Ok(title);
            }
            catch 
            {
                return BadRequest();
            }
        }

        [Produces("application/json")]
        [HttpGet("searchAuthor/author")]
        public async Task<IActionResult> SearchAuthor()
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();
                var author = _context.Books.Where(b => b.Authors.Name.ToLower()
                                .StartsWith(term.ToLower())).Select(b => b.Authors.Name).Distinct().ToList();
                return Ok(author);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
