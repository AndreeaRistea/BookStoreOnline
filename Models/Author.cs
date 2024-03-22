using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Proiect_CE.Models
{
    public class Author
    {
        [Key] public int Id { get; set; }


        [DisplayName("Nume autor")]
        public string Name { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
