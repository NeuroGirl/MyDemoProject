namespace Game.State
{
    using Game.Items;
    using Game.Inventory;
    using System;

    public sealed class InventoryFullState : IInventoryState
    {
        private readonly Inventory inventory;

        public InventoryFullState(Inventory inv)
        {
            inventory = inv;
        }

        public void AddItem(IItem item)
        {
            // In a full inventory we throw – alternative is to put in a stash.
            throw new InvalidOperationException("Inventory is full – cannot add more items.");
        }
    }
}