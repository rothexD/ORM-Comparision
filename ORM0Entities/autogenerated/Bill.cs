﻿using System;
using System.Collections.Generic;

namespace ORM0Entities.autogenerated
{
    public partial class Bill
    {
        public Bill()
        {
            FkArticles = new List<Article>();
        }
        public int Billsid { get; set; }
        public decimal Purchaseprice { get; set; }
        public virtual IList<Article> FkArticles { get; set; }
    }
}
