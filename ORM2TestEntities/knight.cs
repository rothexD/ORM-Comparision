using System.ComponentModel.DataAnnotations;

namespace ORM2TestEntiteis;

public class knight
{
    [Key]
    public int id { get;set; }
    
    public string name { get; set; }

    public virtual weapon weapon { get; set; }
}