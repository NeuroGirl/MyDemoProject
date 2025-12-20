namespace Game.Items
{
    using Game.Equipping;

    public abstract class ItemBase : IItem
    {
        public string Name { get; protected set; } = string.Empty;
        public string Description { get; protected set; } = string.Empty;

        protected readonly IEquipStrategy? equipStrategy;

        protected ItemBase(IEquipStrategy? strategy)
        {
            equipStrategy = strategy;
        }

        public virtual void Use()
        {}

        public virtual void Equip()
        {
            equipStrategy?.Equip(this);
        }

        public virtual bool IsEquipped => false;
    }
}