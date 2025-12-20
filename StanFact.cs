namespace Game.Factory
{
    using Game.Items;

    public sealed class StandardFactory : AbstractFactory
    {
        public override IItemFactory CreateWeaponFactory() => new StandardItemFactory();
        public override IItemFactory CreateArmorFactory() => new StandardItemFactory();
        public override IItemFactory CreatePotionFactory() => new StandardItemFactory();
        public override IItemFactory CreateQuestFactory() => new StandardItemFactory();
    }
}