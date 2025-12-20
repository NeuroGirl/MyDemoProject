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
}}