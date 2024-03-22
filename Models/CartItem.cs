using Proiect_CE.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proiect.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
        //[ForeignKey("ISBN")]
        public string ISBN { get; set; }
        public virtual Book Book { get; set; }
        public int CartId { get; set; }
        [ForeignKey("CartId")]
        public Cart Cart { get; set; }
        [ForeignKey("OrderId")]
        public int? OrderId { get; set; }
        public Order Order { get; set; }
    }
}
