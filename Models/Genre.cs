using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Proiect_CE.Models
{
    public class Genre
    {
        [Key] public int Id { get; set; }
        [DisplayName("Gen carte")]
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Book> Books { get; set; }
    }
}
