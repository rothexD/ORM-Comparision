using System.ComponentModel.DataAnnotations;
using OR_Mapper.Framework;

namespace Orm3TestEntities
{
    public class Bills: Entity
    {
        [Key]
        public int Id { get; set; }
        public decimal Purchaseprice { get; set; }

        public List<Articles> Articles { get; set; } = new List<Articles>();
    }
}
