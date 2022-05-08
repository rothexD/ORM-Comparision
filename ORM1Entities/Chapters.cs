using ORM.Core.Attributes;

namespace ORM1Entities
{
    public partial class Chapters
    {
        [PrimaryKey(autoIncrement:false)]
        public int Id { get; set; }
        public string Chaptername { get; set; } = null!;
        
        [ForeignKey("FK_BookID")]
        public Books Book { get; set; }

        [ForeignKey("fk_chapter_Id")]
        public List<Pages> Pages { get; set; } = new List<Pages>();
    }
}
