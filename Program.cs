namespace DeliverySystem.DeliveryService
{
public class DeliveryService
{
    private static int _nextOrderId = 1000;
    private Order.OrderFactory _factory;
    private CostCals.PriceCalculator _priceCalculator;
    private MenuManager.MenuManager _menuManager = MenuManager.MenuManager.Instance;

    public DeliveryService(Order.OrderFactory factory, CostCals.IPriceCalculationStrategy initialStrategy)
    {
        _factory = factory;
        _priceCalculator = new CostCals.PriceCalculator(initialStrategy);
    }

    public Order.Order CreateAndConfigureOrder(string type, int[] dishIds)
    {
        Order.Order order = type.ToLower() switch
        {
            "vip" => new Order.VIPOrderFactory().CreateOrder(++_nextOrderId),
            _ => new Order.StandardOrderFactory().CreateOrder(++_nextOrderId),
        };

        foreach (var id in dishIds)
        {
            var menuItem = _menuManager.GetItem(id);
            if (menuItem != null)
            {
                order.Items.Add(new OrderComponent.Dish(menuItem));
            }
        }
        
        order.CalculateBaseTotal();
        return order;
    }

    public Order.Order ApplyDecoratorsAndFinalizePrice(Order.Order baseOrder, bool usePremiumPackaging, bool useExpressDelivery)
    {
        Order.Order finalOrder = baseOrder;

        if (usePremiumPackaging)
        {
            finalOrder = new AddProperties.PremiumPackagingDecorator(finalOrder);
        }

        if (useExpressDelivery)
        {
            _priceCalculator.SetStrategy(new CostCals.ExpressPricingStrategy());
        }
        else
        {
            _priceCalculator.SetStrategy(new CostCals.StandardPricingStrategy());
        }

        finalOrder.CalculateBaseTotal(); 
        
        string summary;
        decimal execCost = _priceCalculator.ExecuteCalculation(
        finalOrder.BaseTotal,
        finalOrder,
        out summary
        );

        finalOrder.SetFinalCost(execCost);

        Console.WriteLine($"\n--- Детализация Стоимости Заказа #{finalOrder.OrderId} ({finalOrder.GetType().Name}) ---");
        Console.WriteLine(summary);
        Console.WriteLine($"ИТОГО К ОПЛАТЕ: {finalOrder.FinalCost:C}");
        Console.WriteLine("---\n");
        
        return finalOrder;
    }

    public void ProcessAndTrackOrder(Order.Order order, StateControl.IOrderObserver observer)
    {
        if (order is StateControl.IOrderSubject subject)
        {
            subject.Attach(observer);
        }

        order.ProcessOrder();
        order.Status = Order.OrderStatus.Preparing;
        order.Status = Order.OrderStatus.InTransit;
        order.Status = Order.OrderStatus.Delivered;
        if (order is StateControl.IOrderSubject subjectToDetach)
        {
            subjectToDetach.Detach(observer);
        }
    }

    public Order.Order ApplyAndFinalizePrice(Order.Order order, bool applyTax, bool applyDiscount)
    {
        if (order == null) throw new ArgumentNullException(nameof(order));

        order.CalculateBaseTotal();
            decimal finalPrice = _priceCalculator.ExecuteCalculation(
            order.BaseTotal, 
            order,            
            out _);
        order.SetFinalCost(finalPrice);          
        return order;
    }
}


public class Program
{
    public static void Main()
    {
        var service = new DeliveryService(new Order.StandardOrderFactory(), new CostCals.StandardPricingStrategy());
        var menu = MenuManager.MenuManager.Instance;
        var order1 = service.CreateAndConfigureOrder("standard", new[] { 101, 104 });
        var combo = new OrderComponent.DishComposite("Ланч-набор");
        combo.Add(new OrderComponent.Dish(menu.GetItem(103)));
        combo.Add(new OrderComponent.Dish(menu.GetItem(102)));
        order1.Items.Add(combo);

        Console.WriteLine($"\n--- Начальная конфигурация заказа #{order1.OrderId} ---");
        foreach(var item in order1.Items)
        {
            Console.WriteLine($"- {item.GetDescription()} ({item.GetPrice():C})");
        }
        Console.WriteLine($"Базовая сумма: {order1.BaseTotal:C}");
        
        var finalOrder1 = service.ApplyAndFinalizePrice(order1, false, false);
        var courierA = new StateControl.Courier("Алексей");
        service.ProcessAndTrackOrder(finalOrder1, courierA);
        
        
        Console.WriteLine("\n=======================================================\n");

        var vipOrder = service.CreateAndConfigureOrder("vip", new[] { 102 });
        var finalOrder2 = service.ApplyAndFinalizePrice(vipOrder, true, true);
        var courierB = new StateControl.Courier("Борис");
        service.ProcessAndTrackOrder(finalOrder2, courierB);
    }
}
}