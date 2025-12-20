using Game.Items;
using Game.Equipping;
using Game.Builder;
using Game.Factory;
using Xunit;

namespace Game.Tests
{
    public class InventoryTests
    {
        [Fact]
        public void Add_Item_To_NormalState_ShouldAdd()
        {
            var inv = new Inventory.Inventory(2);
            var sword = new Weapon("Sword", "A sharp blade", 10, new StandardEquipStrategy());

            inv.Add(sword);

            Assert.Single(inv.Items);
            Assert.Contains(sword, inv.Items);
        }

        [Fact]
        public void Add_Items_To_FullInventory_ShouldThrow()
        {

            var inv = new Inventory.Inventory(1);
            var sword = new Weapon("Sword", "A sharp blade", 10, new StandardEquipStrategy());
            var shield = new Weapon("Shield", "Protective gear", 5, new StandardEquipStrategy());
            inv.Add(sword);

            Assert.Throws<InvalidOperationException>(() => inv.Add(shield));
        }

        [Fact]
        public void Remove_Item_ShouldSucceed()
        {

            var inv = new Inventory.Inventory(2);
            var potion = new Potion("Healing", "Restores health", 20);
            inv.Add(potion);

            inv.Remove(potion);

            Assert.Empty(inv.Items);
        }

        [Fact]
        public void BuildWeapon_ShouldReturnWeaponWithCorrectProperties()
        {

            var builder = new ItemBuilder();

            var weapon = builder
                .WithName("Excalibur")
                .WithDescription("Legendary Sword")
                .WithDamage(999)
                .BuildWeapon();

            Assert.Equal("Excalibur", weapon.Name);
            Assert.Equal(999, weapon.Damage);
            Assert.False(weapon.IsEquipped);
        }

        [Fact]
        public void BuildPotion_ShouldThrow_OnZeroHeal()
        {
            var builder = new ItemBuilder();
            Assert.Throws<InvalidOperationException>(() => builder.WithHeal(0).BuildPotion());
        }

        [Fact]
        public void EquipWeaponStrategy_ShouldMarkAsEquipped()
        {
            var builder = new ItemBuilder();
            var weapon = builder
                .WithName("Dagger")
                .WithDamage(20)
                .WithEquipStrategy(EquipWeaponStrategy.Default)
                .BuildWeapon();

            weapon.Equip();

            Assert.True(weapon.IsEquipped);
        }

        [Fact]
        public void StandardFactory_ShouldProduceItems_WithCorrectType()
        {
            var factory = new StandardFactory();

            var weapon = factory.CreateWeaponFactory().CreateWeapon("Axe", 15);
            var armor = factory.CreateArmorFactory().CreateArmor("Chainmail", 25);
            var potion = factory.CreatePotionFactory().CreatePotion("Mana", 30);
            var quest = factory.CreateQuestFactory().CreateQuestItem("Key");

            Assert.IsType<Weapon>(weapon);
            Assert.IsType<Armor>(armor);
            Assert.IsType<Potion>(potion);
            Assert.IsType<QuestItem>(quest);
        }

        [Fact]
        public void EquippingStrategy_Assigned_Correctly()
        {
            var factory = new StandardFactory();
            var weaponFactory = factory.CreateWeaponFactory();

            var weapon = weaponFactory.CreateWeapon("Hammer", 12);
            weapon.Equip();

            Assert.True(weapon.IsEquipped);
        }
    }
}