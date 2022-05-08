using System.ComponentModel.DataAnnotations;
using OR_Mapper.Framework;

namespace Orm3TestEntities
{
    public class Articles : Entity
    { 
    [Key]
    public int Id { get; set; }
    public string Articlename { get; set; } = null!;
    public decimal Articleprice { get; set; }
    public bool? Ishidden { get; set; }

    public List<Bills> Bills { get; set; } = new List<Bills>();
    }
}
