using System;
using System.Collections.Generic;

namespace ORM0Entities
{
    public partial class Chapter
    {
        public Chapter()
        {
            Pages = new HashSet<Page>();
        }

        public int Id { get; set; }
        public string Chaptername { get; set; } = null!;
        public int FkBookid { get; set; }

        public virtual Book FkBook { get; set; } = null!;
        public virtual ICollection<Page> Pages { get; set; }
    }
}
