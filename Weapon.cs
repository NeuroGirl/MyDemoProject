namespace Game.Items
{
    using Game.Equipping;

    public class Weapon : ItemBase, IEquippable
    {
        public int Damage { get; private set; }

        public Weapon(string name, string description, int damage,
            IEquipStrategy strategy) : base(strategy)
        {
            Name = name;
            Description = description;
            Damage = damage;
        }

        public bool IsEquipped { get; private set; }

        public override void Equip()
        {
            IsEquipped = true;
            equipStrategy?.Equip(this);
        }

        public override void Use()
        {}
    }
}