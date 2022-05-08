using System.ComponentModel.DataAnnotations;

namespace ORM2TestEntiteis;

public class articles
{
    [Key]
    public int articlesid { get; set; }
    public string articlename { get; set; } = null!;
    public decimal articleprice { get; set; }
    public bool? ishidden { get; set; }

    public virtual List<bills> Bills { get; set; } = new();
}