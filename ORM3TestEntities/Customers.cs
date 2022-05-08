using System.ComponentModel.DataAnnotations;
using OR_Mapper.Framework;

namespace Orm3TestEntities
{
    public class Customers: Entity
    {
        [Key]
        public int Id { get; set; }
        public string Lastname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool Isvip { get; set; }
        public bool? Customerlikescolorgreen { get; set; }
        public bool? Customerlikescars { get; set; }
        public int Totalarticlesaddedtoshoppingcart { get; set; }
        public decimal Defaultpurchasepricemultiplicator { get; set; }
    }
}
