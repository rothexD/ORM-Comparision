using System.ComponentModel.DataAnnotations;
using OR_Mapper.Framework;

namespace Orm3TestEntities
{
    public class Pages: Entity
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public Chapters Chapter { get; set; }
    }
}
