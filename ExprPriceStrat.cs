namespace DeliverySystem.CostCals
{
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
}