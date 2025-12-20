namespace Game.Items
{
    public interface IItem
    {
        string Name { get; }
        string Description { get; }

        void Use();        
        void Equip();      
        bool IsEquipped { get; }
    }
}