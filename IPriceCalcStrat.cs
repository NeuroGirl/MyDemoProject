namespace DeliverySystem.CostCals
{
public interface IPriceCalculationStrategy
{
    decimal Calculate(decimal baseTotal, Order.Order order, out string description);
}
}