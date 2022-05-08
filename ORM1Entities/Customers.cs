using System;
using System.Collections.Generic;
using ORM.Core.Attributes;

namespace ORM1Entities
{
    public class Customers
    {
        [PrimaryKey(autoIncrement:false)]
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