using System;
using System.Collections.Generic;

namespace ORM0Entities
{
    public partial class Book
    {
        public Book()
        {
            Chapters = new HashSet<Chapter>();
        }

        public int Id { get; set; }
        public string Bookname { get; set; } = null!;
        public decimal Price { get; set; }
        public string Authorname { get; set; } = null!;

        public virtual ICollection<Chapter> Chapters { get; set; }
    }
}
