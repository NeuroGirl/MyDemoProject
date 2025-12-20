namespace DeliverySystem.StateControl
{
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
    }}}