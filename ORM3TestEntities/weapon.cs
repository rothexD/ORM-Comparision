using System.ComponentModel.DataAnnotations;
using OR_Mapper.Framework;

namespace Orm3TestEntities;

public class weapon: Entity
{
    [Key()]
    public int id { get; set; }
    public string weaponname { get; set; }
    public int damage { get; set; }
    
    public knight Knight { get; set; }
}