namespace Game.Items
{
    public interface IEquippable
    {
        void Equip();
        bool IsEquipped { get; }
    }
}