using Game.Items;
using Game.Equipping;

namespace Game.Tests;
{
public class InventoryTests
{
    [Fact]
    public void Add_Item_To_NormalState_ShouldAdd()
    {
        // Arrange
        var inv = new Inventory.Inventory(2);
        var sword = new Weapon("Sword", "A sharp blade", 10, new StandardEquipStrategy());

        // Act
        inv.Add(sword);

        // Assert
        Assert.Single(inv.Items);
        Assert.Contains(sword, inv.Items);
    }

    [Fact]
    public void Add_Items_To_FullInventory_ShouldThrow()
    {
        // Arrange
        var inv = new Inventory(1);
        var sword = new Weapon("Sword", "A sharp blade", 10, new StandardEquipStrategy());
        var shield = new Weapon("Shield", "Protective gear", 5, new StandardEquipStrategy());
        inv.Add(sword);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => inv.Add(shield));
    }

    [Fact]
    public void Remove_Item_ShouldSucceed()
    {
        // Arrange
        var inv = new Inventory(2);
        var potion = new Potion("Healing", "Restores health", 20);
        inv.Add(potion);

        // Act
        inv.Remove(potion);

        // Assert
        Assert.Empty(inv.Items);
    }

 [Fact]
    public void BuildWeapon_ShouldReturnWeaponWithCorrectProperties()
    {
        // Arrange
        var builder = new ItemBuilder();

        // Act
        var weapon = builder
            .WithName("Excalibur")
            .WithDescription("Legendary Sword")
            .WithDamage(999)
            .BuildWeapon();

        // Assert
        Assert.Equal("Excalibur", weapon.Name);
        Assert.Equal(999, weapon.Damage);
        Assert.False(weapon.IsEquipped);
    }

    [Fact]
    public void BuildPotion_ShouldThrow_OnZeroHeal()
    {
        // Arrange
        var builder = new ItemBuilder();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => builder.WithHeal(0).BuildPotion());
    }

    [Fact]
    public void EquipWeaponStrategy_ShouldMarkAsEquipped()
    {
        // Arrange
        var builder = new ItemBuilder();
        var weapon = builder
            .WithName("Dagger")
            .WithDamage(20)
            .WithEquipStrategy(EquipWeaponStrategy.Default)
            .BuildWeapon();

        // Act
        weapon.Equip();

        // Assert
        Assert.True(weapon.IsEquipped);
    }

    [Fact]
    public void StandardFactory_ShouldProduceItems_WithCorrectType()
    {
        // Arrange
        var factory = new StandardFactory();

        // Act
        var weapon = factory.CreateWeaponFactory().CreateWeapon("Axe", 15);
        var armor = factory.CreateArmorFactory().CreateArmor("Chainmail", 25);
        var potion = factory.CreatePotionFactory().CreatePotion("Mana", 30);
        var quest = factory.CreateQuestFactory().CreateQuestItem("Key");

        // Assert
        Assert.IsType<Weapon>(weapon);
        Assert.IsType<Armor>(armor);
        Assert.IsType<Potion>(potion);
        Assert.IsType<QuestItem>(quest);
    }

    [Fact]
    public void EquippingStrategy_Assigned_Correctly()
    {
        // Arrange
        var factory = new StandardFactory();
        var weaponFactory = factory.CreateWeaponFactory();

        // Act
        var weapon = weaponFactory.CreateWeapon("Hammer", 12);
        weapon.Equip();

        // Assert
        Assert.True(weapon.IsEquipped);
    }
}