using System.ComponentModel.DataAnnotations;

namespace ORM2TestEntiteis;

public class weapon
{
    [Key]
    public int id { get; set; }
    public string weaponname { get; set; }
    public int damage { get; set; }
    
    public virtual knight knight { get; set; }
}