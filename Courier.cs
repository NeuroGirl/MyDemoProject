namespace DeliverySystem.StateControl
{
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