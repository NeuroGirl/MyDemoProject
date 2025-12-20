namespace DeliverySystem.AddProperties
{
public abstract class OrderDecorator : Order.Order
{
    protected Order.Order _wrappedOrder;

    public OrderDecorator(Order.Order wrappedOrder) : base(wrappedOrder.OrderId)
    {
        
        _wrappedOrder = wrappedOrder;
        Items.AddRange(wrappedOrder.Items);
        Status = wrappedOrder.Status;
    }

    public override void CalculateBaseTotal() => _wrappedOrder.CalculateBaseTotal();
    public override void ProcessOrder() => _wrappedOrder.ProcessOrder();
}

public class PremiumPackagingDecorator : OrderDecorator
{
    private const decimal PackagingCost = 75.00m;
    
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