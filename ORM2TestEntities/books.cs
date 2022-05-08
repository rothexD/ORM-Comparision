using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORM2TestEntiteis
{
    public class books
    {
        [Key]
        public int bookid { get; set; }
        public string bookname { get; set; } = null!;
        public decimal price { get; set; }
        public string authorname { get; set; } = null!;
        public virtual List<chapters> chapters { get; set; }
    }
}
