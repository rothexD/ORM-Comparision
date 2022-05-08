using System;
using System.Collections.Generic;

namespace ORM0Entities
{
    public partial class Article
    {
        public Article()
        {
            FkBills = new HashSet<Bill>();
        }

        public int Id { get; set; }
        public string Articlename { get; set; } = null!;
        public decimal Articleprice { get; set; }
        public bool? Ishidden { get; set; }

        public virtual ICollection<Bill> FkBills { get; set; }
    }
}
