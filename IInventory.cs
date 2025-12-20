namespace Game.Inventory
{
    using Game.Items;
    using System.Collections.Generic;

    public interface IInventory
    {
        int Capacity { get; }
        List<IItem> Items { get; }
        void Add(IItem item);
        void Remove(IItem item);
        bool Contains(IItem item);
    }
}