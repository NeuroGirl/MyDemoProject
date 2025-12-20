namespace Game.Items
{
    public class Potion : ItemBase, IUseable
    {
        public int HealAmount { get; private set; }

        public Potion(string name, string description, int healAmount) : base(null)
        {
            Name = name;
            Description = description;
            HealAmount = healAmount;
        }

        public override void Use()
        {
        }
    }
}