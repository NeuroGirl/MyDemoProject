namespace Game.Factory
{
    using Game.Equipping;
    using Game.Items;
    using System;

    public sealed class StandardItemFactory : IItemFactory
    {
        public Weapon CreateWeapon(string name, int damage)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            return new Weapon(name, $"A standard weapon: {name}", damage,
                EquipWeaponStrategy.Default);
        }

        public Armor CreateArmor(string name, int defense)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            return new Armor(name, $"A sturdy armor: {name}", defense,
                EquipArmorStrategy.Default);
        }

        public Potion CreatePotion(string name, int heal)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            return new Potion(name, $"A healing potion: {name}", heal);
        }

        public QuestItem CreateQuestItem(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            return new QuestItem(name, $"Quest item: {name}");
        }
    }
}