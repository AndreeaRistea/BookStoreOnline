using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Proiect_CE.Models
{
    public class PublishingHouse
    {
        [Key] public int Id { get; set; }
        [DisplayName("Editura")]
        public string Name { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
