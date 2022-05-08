using System.ComponentModel.DataAnnotations;
using OR_Mapper.Framework;

namespace Orm3TestEntities;

public class knight : Entity
{
    [Key]
    public int id { get;set; }
    
    public string name { get; set; }

    public weapon weapon { get; set; }
}