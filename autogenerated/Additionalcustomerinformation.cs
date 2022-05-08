using System;
using System.Collections.Generic;

namespace ORM0Entities
{
    public partial class Additionalcustomerinformation
    {
        public Additionalcustomerinformation()
        {
            Customers = new HashSet<Customer>();
        }

        public int Id { get; set; }
        public bool? Customerlikescolorgreen { get; set; }
        public bool? Customerlikescars { get; set; }
        public int Totalarticlesaddedtoshoppingcart { get; set; }
        public decimal Defaultpurchasepricemultiplicator { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}
