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

public class StandardOrder : Order
{
        private decimal baseTotal;

        public StandardOrder(int orderId) : base(orderId)
        {
        }

        public StandardOrder(decimal baseTotal, int orderId): base(orderId)
        {
            this.baseTotal = baseTotal;
        }

        public StandardOrder(object orderId1, int orderId) : base(orderId)
        {}

        public override void CalculateBaseTotal()
    {
        baseTotal = Items.Sum(i => i.GetPrice());
    }
}

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