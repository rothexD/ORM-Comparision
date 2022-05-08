using System.ComponentModel.DataAnnotations;

namespace ORM2TestEntiteis
{
    public class pages
    {
        [Key]
        public int pagesid { get; set; }
        public virtual chapters fk_chapter_Id { get; set; }
        public string text { get; set; } = null!;
        
    }
}
