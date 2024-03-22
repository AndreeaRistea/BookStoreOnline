using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect_CE.Models
{
    public class WishList
    {
        [Key]
        public int id { get; set; }

        public ICollection<Book> books { get; set; } = new List<Book>();
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
