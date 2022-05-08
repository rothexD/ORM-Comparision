using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ORM.Core.Models;

namespace ORM2TestEntiteis
{
    public class chapters
    {
        [Key]
        public int chapterid { get; set; }
        public string chaptername { get; set; } = null!;
        
        public virtual books fk_books_bookid { get; set; }
        public virtual List<pages> Pages { get; set; }
    }
}
