using Proiect_CE.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal TotalAmount
        {
            get
            {
                return CartItems.Sum(item => item.Quantity * item.Book.Price);
            }
        }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User User { get; set; }
        [ForeignKey("OrderId")]
        public int? OrderId { get; set; }
        public Order Order { get; set; }
    }
}
