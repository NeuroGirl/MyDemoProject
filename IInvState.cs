namespace Game.State
{
    using Game.Items;
    using Game.Inventory;
    using System.Collections.Generic;

    public interface IInventoryState
    {
        void AddItem(IItem item);
    }
}