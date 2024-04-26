using Proiect_CE.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Proiect.Models
{
    public class Specification
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("ID carte")]
        [ForeignKey("Book")]
        public string BookId { get; set; }

        [DisplayName("Nume specificatie")]
        public string Name { get; set; }

        [DisplayName("Valoare specificatie")]
        public string Value { get; set; }

        public virtual Book Book { get; set; }
    }
}
