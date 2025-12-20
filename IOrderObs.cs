namespace DeliverySystem.StateControl
{
public interface IOrderObserver
{
    void Update(Order.Order order);
}
}