using ORM.Core.Attributes;

namespace ORM1Entities;

public class Knight
{
    [PrimaryKey(false)]
    public int Id { get;set; }
    
    public string Name { get; set; }
    
   // public int fk_WeaponId { get; set; }
    
    [ForeignKey("fk_WeaponId")]
    public Weapon Weapon { get; set; }
}