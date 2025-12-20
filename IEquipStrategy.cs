namespace Game.Equipping
{
    using Game.Items;

    public interface IEquipStrategy
    {
        void Equip(IItem item);
    }
}