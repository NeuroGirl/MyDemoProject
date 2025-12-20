namespace DeliverySystem.Order
{
public abstract class Order
{
    public int OrderId { get; }
    public Order(int orderId)
    {
        OrderId = orderId;
    }
    public List<OrderComponent.OrderComponent> Items { get; } = new List<OrderComponent.OrderComponent>();
    public decimal BaseTotal => _baseTotal;
    private decimal _baseTotal; 
    private decimal _finalCost; 
    public decimal FinalCost => _finalCost; 
    internal void SetFinalCost(decimal value) => _finalCost = value;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public abstract void CalculateBaseTotal();

    public virtual void ProcessOrder() 
    {
        CalculateBaseTotal();
    }
}

public enum OrderStatus { Pending, Preparing, InTransit, Delivered, Cancelled }
}