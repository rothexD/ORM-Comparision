using ORM.Core.Attributes;

namespace ORM1Entities;

public class Weapon
{
    [PrimaryKey(autoIncrement:false)]
    public int Id { get; set; }
    public string WeaponName { get; set; }
    public int Damage { get; set; }
    
    [ForeignKey("fk_KnightId")]
    public Knight Knight { get; set; }
}