using ORM.Core.Attributes;

namespace ORM1Entities
{
    public partial class Pages
    {
        [PrimaryKey(autoIncrement:false)]
        public int Id { get; set; }
        
        [ForeignKey("fk_chapter_Id")]
        public Chapters Chapter { get; set; }
        public string Text { get; set; } = null!;
    }
}
