namespace DeliverySystem.StateControl
{
public interface IOrderObserver
{
    void Update(Order.Order order);
}

public interface IOrderSubject
{
    void Attach(IOrderObserver observer);
    void Detach(IOrderObserver observer);
    void Notify();
}

public class OrderSubject : Order.Order, IOrderSubject
{
    private List<IOrderObserver> _observers = new List<IOrderObserver>();
    public OrderSubject(int orderId) : base(orderId)
        {}
    public override void CalculateBaseTotal()
    {}
    public void Attach(IOrderObserver observer)
    {
        Console.WriteLine($"[Observer] Наблюдатель {observer.GetType().Name} прикреплен.");
        _observers.Add(observer);
    }

    public void Detach(IOrderObserver observer)
    {
        _observers.Remove(observer);
    }

    public void Notify()
    {
        Console.WriteLine($"[Observer] Уведомление всех наблюдателей об изменении статуса до {Status}.");
        foreach (var observer in _observers)
        {
            observer.Update(this);
        }
    }

    public new Order.OrderStatus Status
    {
        get => base.Status;
        set
        {
            if (base.Status != value)
            {
                base.Status = value;
                Notify();
            }
        }
    }
}

public class Courier : IOrderObserver
{
    private string _name;
    public Courier(string name) { _name = name; }

    public void Update(Order.Order order)
    {
        if (order.Status == Order.OrderStatus.InTransit)
        {
            Console.WriteLine($"[Courier {_name}]: Заказ #{order.OrderId} готов к забору (Статус: {order.Status}). Приступаю к доставке!");
        }
        else if (order.Status == Order.OrderStatus.Delivered)
        {
            Console.WriteLine($"[Courier {_name}]: Заказ #{order.OrderId} доставлен. Завершаю смену.");
        }
    }
}
}