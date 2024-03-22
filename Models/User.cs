using Microsoft.AspNetCore.Identity;
using Proiect.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect_CE.Models
{
    public class User
    {
        [Key]
        public int id { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }

        public ICollection<Book> Books { get; set; }
        public int? WishListId { get; set; }
        public WishList? WishList { get; set; }
        public int? CartId { get; set; }
        public Cart? Cart { get; set; }
        [ForeignKey("InnerUserId")]
        public Guid InnerUserId { get; set; }
        public IdentityUser InnerUser { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
