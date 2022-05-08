using System.ComponentModel.DataAnnotations;
using OR_Mapper.Framework;

namespace Orm3TestEntities
{
    public class Chapters : Entity
    {
        [Key]
        public int Id { get; set; }
        public string Chaptername { get; set; } = null!;
        
        public Books Book { get; set; }
        
        public List<Pages> Pages { get; set; }
    }
}
