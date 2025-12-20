namespace Game.Items
{
    using Game.Equipping;

    public class Armor : ItemBase, IEquippable
    {
        public int Defense { get; private set; }

        public Armor(string name, string description, int defense,
            IEquipStrategy strategy) : base(strategy)
        {
            Name = name;
            Description = description;
            Defense = defense;
        }

        public bool IsEquipped { get; private set; }

        public override void Equip()
        {
            IsEquipped = true;
            equipStrategy?.Equip(this);
        }

        public override void Use()
        {
        }
    }
}