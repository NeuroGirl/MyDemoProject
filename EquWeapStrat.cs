namespace Game.Equipping
{
    using Game.Items;
    public sealed class EquipWeaponStrategy : IEquipStrategy
    {
        public void Equip(IItem item)
        {
        }

        public static EquipWeaponStrategy Default => new EquipWeaponStrategy();
    }
}