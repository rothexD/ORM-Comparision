﻿using System;
using System.Collections.Generic;

namespace ORM0Entities.autogenerated
{
    public partial class Book
    {
        public Book()
        {
            Chapters = new List<Chapter>();
        }
        public int Bookid { get; set; }
        public string Bookname { get; set; } = null!;
        public decimal Price { get; set; }
        public string Authorname { get; set; } = null!;
        public virtual IList<Chapter> Chapters { get; set; }
    }
}
