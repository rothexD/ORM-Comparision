using System.ComponentModel.DataAnnotations;
using OR_Mapper.Framework;

namespace Orm3TestEntities
{
    public class Books: Entity
    {
        [Key]
        public int Id { get; set; }
        public string Bookname { get; set; } = null!;
        public decimal Price { get; set; }
        public string Authorname { get; set; } = null!;
        
        public List<Chapters> Chapters { get; set; }
    }
}
