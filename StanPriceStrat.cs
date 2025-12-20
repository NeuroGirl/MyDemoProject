namespace DeliverySystem.CostCals
{
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
}