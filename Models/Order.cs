using Proiect_CE.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect.Models
{
    public class Order
    {
        /*public int OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public ICollection<Book> Books { get; set; }
        public int TotalCount { get; set; }
        public DateTime OrderDate { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User User { get; set; }
        */
        [Key]
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public decimal TotalAmount
        {
            get
            {
                return OrderItems.Sum(item => item.Quantity * item.Book.Price);
            }
        }

    }
}
