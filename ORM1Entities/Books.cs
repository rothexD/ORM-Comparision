using ORM.Core.Attributes;

namespace ORM1Entities
{
    public partial class Books
    {
        [PrimaryKey(autoIncrement:false)]
        public int Id { get; set; }
        public string Bookname { get; set; } = null!;
        public decimal Price { get; set; }
        public string Authorname { get; set; } = null!;

        [ForeignKey("FK_BookID")]
        public List<Chapters> Chapter { get; set; } = new List<Chapters>();
    }
}
