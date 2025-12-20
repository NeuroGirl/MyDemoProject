namespace DeliverySystem.AddProperties
{
public class PremiumPackagingDecorator : OrderDecorator
{
    private const decimal PackagingCost = 75.25m;
    
    public PremiumPackagingDecorator(Order.Order wrappedOrder) : base(wrappedOrder)
    {
       wrappedOrder.Items.Add(new OrderComponent.Dish(
            new MenuItem.MenuItem(999, "Премиум Упаковка", PackagingCost)));
    }
    public override void CalculateBaseTotal()
    {
        base.CalculateBaseTotal();
    }

    public override string ToString() => $"{base.ToString()} + Премиум Упаковка";
}
}