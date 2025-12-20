namespace Game.Builder
{
    using Game.Equipping;
    using Game.Items;

    public sealed class ItemBuilder
    {
        private string name = "Unnamed";
        private string description = "No description.";
        private int damage = 0;
        private int defense = 0;
        private int heal = 0;
        private IEquipStrategy? equipStrategy;

        public ItemBuilder WithName(string name)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            return this;
        }

        public ItemBuilder WithDescription(string description)
        {
            this.description = description ?? throw new ArgumentNullException(nameof(description));
            return this;
        }

        public ItemBuilder WithEquipStrategy(IEquipStrategy strategy)
        {
            equipStrategy = strategy;
            return this;
        }

        public ItemBuilder WithDamage(int dmg)
        {
            damage = dmg;
            return this;
        }

        public ItemBuilder WithDefense(int def)
        {
            defense = def;
            return this;
        }

        public ItemBuilder WithHeal(int heal)
        {
            this.heal = heal;
            return this;
        }

        public Weapon BuildWeapon()
        {
            if (damage <= 0) throw new InvalidOperationException("Weapon must have damage > 0.");
            return new Weapon(name, description, damage, equipStrategy ?? new StandardEquipStrategy());
        }

        public Armor BuildArmor()
        {
            if (defense <= 0) throw new InvalidOperationException("Armor must have defense > 0.");
            return new Armor(name, description, defense, equipStrategy ?? new StandardEquipStrategy());
        }

        public Potion BuildPotion()
        {
            if (heal <= 0) throw new InvalidOperationException("Potion must heal > 0.");
            return new Potion(name, description, heal);
        }

        public QuestItem BuildQuestItem()
        {
            return new QuestItem(name, description);
        }
    }
}