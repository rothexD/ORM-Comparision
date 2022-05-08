using System;
using System.Collections.Generic;

namespace ORM0Entities
{
    public partial class Page
    {
        public int Id { get; set; }
        public int FkChapterId { get; set; }
        public string Text { get; set; } = null!;

        public virtual Chapter FkChapter { get; set; } = null!;
    }
}
