using System;
using System.Collections.Generic;

namespace ORM0Entities
{
    public partial class Bill
    {
        public Bill()
        {
            FkArticles = new HashSet<Article>();
        }

        public int Id { get; set; }
        public decimal Purchaseprice { get; set; }
        public DateTime Purchasedate { get; set; }

        public virtual ICollection<Article> FkArticles { get; set; }
    }
}
