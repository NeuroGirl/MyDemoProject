namespace DeliverySystem.CostCals
{
public interface IPriceCalculationStrategy
{
    decimal Calculate(decimal baseTotal, Order.Order order, out string description);
}

public class StandardPricingStrategy : IPriceCalculationStrategy
{
    private const decimal DeliveryFee = 150.00m;
    private const decimal TaxRate = 0.05m;

    public decimal Calculate(decimal baseTotal, Order.Order order, out string description)
    {
        var tax = baseTotal * TaxRate;
        var finalCost = baseTotal + tax + DeliveryFee;
        description = $"База: {baseTotal:C} + Налог (5%): {tax:C} + Доставка: {DeliveryFee:C}";
        return finalCost;
    }
}

public class ExpressPricingStrategy : IPriceCalculationStrategy
{
    private const decimal ExpressFee = 350.00m;
    private const decimal Discount = 50.00m;

    public decimal Calculate(decimal baseTotal, Order.Order order, out string description)
    {
        var totalAfterDiscount = baseTotal - Discount;
        var finalCost = totalAfterDiscount + ExpressFee;
        description = $"База: {baseTotal:C} - Скидка: {Discount:C} + Срочная Доставка: {ExpressFee:C}";
        return finalCost;
    }
}

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