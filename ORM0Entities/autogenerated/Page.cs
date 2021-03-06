using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ORM0Entities.autogenerated
{
    public partial class Page
    {
        public int Pagesid { get; set; }
        public int FkChapterId { get; set; }
        public string Text { get; set; } = null!;
        public virtual Chapter FkChapter { get; set; } = null!;
    }
}
