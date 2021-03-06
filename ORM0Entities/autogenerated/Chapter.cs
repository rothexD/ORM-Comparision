using System;
using System.Collections.Generic;

namespace ORM0Entities.autogenerated
{
    public partial class Chapter
    {
        public Chapter()
        {
            Pages = new List<Page>();
        }
        public int Chapterid { get; set; }
        public string Chaptername { get; set; } = null!;
        public int FkBookid { get; set; }
        public virtual Book FkBook { get; set; } = null!;
        public virtual IList<Page> Pages { get; set; }
    }
}
