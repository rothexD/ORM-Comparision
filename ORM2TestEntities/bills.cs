

using System.ComponentModel.DataAnnotations;

namespace ORM2TestEntiteis
{
    public class bills
    {
        [Key]
        public int billsid { get; set; }
        public decimal purchaseprice { get; set; }
        public virtual List<articles> articles { get; set; } = new List<articles>();
    }
}
