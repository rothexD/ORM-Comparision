﻿namespace ORM0Entities.autogenerated
{
    public partial class Weapon
    {
        public int Id { get; set; }
        public string? Weaponname { get; set; }
        public int? Damage { get; set; }

        public int? FkKnightId { get; set; }
        public virtual Knight Knight { get; set; }
    }
}
