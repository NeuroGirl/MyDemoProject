using Game.Items;

namespace Game.State
{
    public sealed class InventoryNormalState : IInventoryState
    {
        private readonly Inventory.Inventory inventory;

        public InventoryNormalState(Inventory.Inventory inv) => inventory = inv;

        public void AddItem(IItem item)
        {
            inventory.InternalList.Add(item);

            if (inventory.IsFull)
            {
                inventory.State = new InventoryFullState(inventory);
            }
        }
    }
}