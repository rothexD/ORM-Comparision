using ORM.Core.Attributes;

namespace ORM1Entities
{
    public partial class Bills
    {
        [PrimaryKey(autoIncrement:false)]
        public int Id { get; set; }

        public decimal Purchaseprice { get; set; }

        [ManyToMany("BillsArticles", "Bills_id_fk", "articles_id_fk")]
        public List<Articles> FkArticles { get; set; } = new List<Articles>();
    }
}
