using Proiect.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Proiect_CE.Models
{
    public class Book
    {
        [DisplayName("ISBN")]
        [Key] public string ISBN { get; set; }
        [DisplayName("Titlu carte")]
        public string Title { get; set; }
        [DisplayName("Descriere")]
        public string Description { get; set; }
        [DisplayName("Editura")]
        public virtual PublishingHouse PublishingHouse { get; set; }
        [DisplayName("Editura")]
        public int PublishingHouseId { get; set; }
        [DisplayName("Pret")]
        public int Price { get; set; }
        [DisplayName("Data publicarii")]
        public DateTime DatePublishing { get; set; }
        //[DisplayName("Specificatii")]
        //public string Specifications { get; set; }
        [DisplayName("Gen carte")]
        public virtual Genre Genre { get; set; }
        [DisplayName("Gen carte")]
        public int GenreId { get; set; }
        [DisplayName("Autor")]
        public virtual Author Authors { get; set; }
        [DisplayName("Autor")]
        public int AuthorId { get; set; }
        [DisplayName("Stoc disponibil")]
        public int Stock { get; set; }
        public byte[]? Image { get; set; }

        [DataType(DataType.Upload)]
        [DisplayName("Imagine")]
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public byte[]? FileText { get; set; }

        [DataType(DataType.Upload)]
        [DisplayName("Fragmente")]
        [NotMapped]
        public IFormFile FragmentFile { get; set; }
        public string FragmentFileName { get; set; }
        public ICollection<WishList> WishLists { get; set; }
        //public ICollection<Cart> Carts { get; set; }
        public CartItem CartItem { get; set; }
        public string Author { get; }
        public string Genre1 { get; }
        [DisplayName("Specificatii")]
        public ICollection<Specification> Specifications { get; set; }

    }
}
