namespace Game.Factory
{
    using Game.Items;

    public interface IItemFactory
    {
        Weapon CreateWeapon(string name, int damage);
        Armor CreateArmor(string name, int defense);
        Potion CreatePotion(string name, int heal);
        QuestItem CreateQuestItem(string name);
    }
}