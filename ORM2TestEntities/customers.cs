using System.ComponentModel.DataAnnotations;

namespace ORM2TestEntiteis
{
    public class customers
    {
        [Key]
        public int customerid { get; set; }
        public string lastname { get; set; } = null!;
        public string email { get; set; } = null!;
        public bool isvip { get; set; }
        public bool? customerlikescolorgreen { get; set; }
        public bool? customerlikescars { get; set; }
        public int totalarticlesaddedtoshoppingcart { get; set; }
        public decimal defaultpurchasepricemultiplicator { get; set; }
    }
}
