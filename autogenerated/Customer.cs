using System;
using System.Collections.Generic;

namespace ORM0Entities
{
    public partial class Customer
    {
        public int Id { get; set; }
        public int FkAdditionalcustomerinformation { get; set; }
        public string Lastname { get; set; } = null!;
        public DateTime CustomerSince { get; set; }
        public string Email { get; set; } = null!;
        public bool Isvip { get; set; }

        public virtual Additionalcustomerinformation FkAdditionalcustomerinformationNavigation { get; set; } = null!;
    }
}
