namespace DeliverySystem.CostCals
{
public class PriceCalculator
{
    private IPriceCalculationStrategy _strategy;

    public PriceCalculator(IPriceCalculationStrategy strategy)
    {
        _strategy = strategy;
    }

    public void SetStrategy(IPriceCalculationStrategy strategy)
    {
        _strategy = strategy;
    }

    public decimal ExecuteCalculation(decimal baseTotal, Order.Order order, out string summary)
    {
        return _strategy.Calculate(baseTotal, order, out summary);
    }
}
}