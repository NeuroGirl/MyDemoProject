namespace Game.Factory
{
    public abstract class AbstractFactory
    {
        public abstract IItemFactory CreateWeaponFactory();
        public abstract IItemFactory CreateArmorFactory();
        public abstract IItemFactory CreatePotionFactory();
        public abstract IItemFactory CreateQuestFactory();
    }
}