namespace DeliverySystem.Order
{
public class VIPOrder : StandardOrder
{
    public VIPOrder(int orderId) : base(orderId)
        {}
    public override void ProcessOrder()
    {
        base.ProcessOrder();
    }
}

public abstract class OrderFactory
{
    public abstract Order CreateOrder(int orderId);
}

public class StandardOrderFactory : OrderFactory
{
    public override Order CreateOrder(int orderId) => new StandardOrder(orderId);
}

public class VIPOrderFactory : OrderFactory
{
    public override Order CreateOrder(int orderId) => new VIPOrder(orderId);
}
}