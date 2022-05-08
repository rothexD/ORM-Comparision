using ORM.Core.Attributes;

namespace ORM1Entities
{
    public partial class Articles
    {
        [PrimaryKey(autoIncrement:false)]
        public int Id { get; set; }
        public string Articlename { get; set; } = null!;
        public decimal Articleprice { get; set; }
        public bool? Ishidden { get; set; }

        [ManyToMany("BillsArticles", "articles_id_fk", "Bills_id_fk")]
        public List<Bills> FkBills { get; set; } = new List<Bills>();
    }
}
