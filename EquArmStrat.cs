namespace Game.Equipping
{
    using Game.Items;
    public sealed class EquipArmorStrategy : IEquipStrategy
    {
        public void Equip(IItem item)
        {}

        public static EquipArmorStrategy Default => new EquipArmorStrategy();
    }
}