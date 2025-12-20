using Game.Items;
using Game.State;

namespace Game.Inventory
{
    public sealed class Inventory : IInventory
    {

        public IReadOnlyList<IItem> Items => items.AsReadOnly();

        public int Capacity { get; }

        public bool IsFull => items.Count >= Capacity;

        internal List<IItem> InternalList => items;

        private readonly List<IItem> items = new List<IItem>();

        internal IInventoryState State { get; set; }

        List<IItem> IInventory.Items => throw new NotImplementedException();

        public Inventory(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            Capacity = capacity;
            State = new InventoryNormalState(this);
        }

        public void Add(IItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            State.AddItem(item);
        }

        public void Remove(IItem item)
        {
            if (!items.Remove(item))
                throw new InvalidOperationException("Item not found in inventory.");
        }

        public bool Contains(IItem item) => items.Contains(item);
    }
}