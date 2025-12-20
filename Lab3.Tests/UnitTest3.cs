namespace DeliverySystem.Tests
{
public class DeliveryServiceTests
{
    [Fact]
    public void MenuManager_ShouldBeSingleton()
    {
        var instance1 = MenuManager.MenuManager.Instance;
        var instance2 = MenuManager.MenuManager.Instance;
        
        Assert.Same(instance1, instance2);
    }

    [Theory]
    [InlineData("standard")]
    [InlineData("vip")]
    public void OrderFactory_ShouldCreateCorrectOrderType(string type)
    {
        var factory = type.ToLower() == "vip" ? new Order.VIPOrderFactory() : (Order.OrderFactory) new  Order.StandardOrderFactory();
        var order = factory.CreateOrder(1);

        if (type.ToLower() == "vip")
        {
            Assert.IsType<Order.VIPOrder>(order);
        }
        else
        {
            Assert.IsType<Order.StandardOrder>(order);
        }
    }

    // Composite тест: проверяем суммирование компонентов
    [Fact]
    public void DishComposite_ShouldCalculateTotalPriceCorrectly()
    {
        var menu = MenuManager.MenuManager.Instance;
        var burger = new OrderComponent.Dish(menu.GetItem(101)); // 450.00
        var fries = new OrderComponent.Dish(menu.GetItem(104));  // 150.00
        
        var combo = new OrderComponent.DishComposite("Test Combo");
        combo.Add(burger);
        combo.Add(fries);

        Assert.Equal(600.00m, combo.GetPrice());
        Assert.Contains("Бургер Классический", combo.GetDescription());
    }

    // Strategy & Pricing Тесты
    [Theory]
    [InlineData(1000.00, false, 1000.00 + 50.00 + 150.00)] // Base + Tax(5%) + Delivery
    [InlineData(1000.00, true, 1000.00 - 50.00 + 350.00)]  // Base - Discount + Express Fee
    public void PriceCalculator_ShouldApplyStrategyCorrectly(decimal baseTotal, bool isExpress, decimal expectedTotal)
    {
        
        var orderPlaceholder = new Order.StandardOrder (0);
        
        CostCals.IPriceCalculationStrategy strategy = isExpress 
            ? new CostCals.ExpressPricingStrategy() 
            : new CostCals.StandardPricingStrategy();
            
        var calculator = new CostCals.PriceCalculator(strategy);
        string summary;
        
        var result = calculator.ExecuteCalculation(baseTotal, orderPlaceholder, out summary);

        Assert.Equal(expectedTotal, result);
    }
    
    // Decorator Тесты
    [Fact]
    public void PremiumDecorator_ShouldIncreaseCost()
    {
        var menu = MenuManager.MenuManager.Instance;
        var standardOrder = new Order.StandardOrder(0);
        standardOrder.Items.Add(new OrderComponent.Dish(menu.GetItem(101))); // 450.00
        standardOrder.CalculateBaseTotal(); // BaseTotal = 450.00

        var decoratedOrder = new AddProperties.PremiumPackagingDecorator(standardOrder);
        decoratedOrder.CalculateBaseTotal(); // BaseTotal должен быть 450 + 75 = 525.00
        
        Assert.Equal(525.00m, decoratedOrder.BaseTotal);
    }
    
    // Observer Тест: Проверяем, что уведомление срабатывает при смене статуса
    [Fact]
    public void Order_ShouldNotifyObserversOnStatusChange()
    {
        var mockObserver = new MockCourierObserver();
        var order = new StateControl.OrderSubject(100);
        order.Attach(mockObserver);
        
        order.Status = Order.OrderStatus.InTransit; // Триггер
        
        // Проверяем, что метод Update был вызван
        Assert.True(mockObserver.UpdateCalled);
        Assert.Equal(Order.OrderStatus.InTransit, order.Status);
        
        // Убедимся, что при повторной установке того же статуса уведомление не срабатывает
        mockObserver.Reset();
        order.Status = Order.OrderStatus.InTransit;
        Assert.False(mockObserver.UpdateCalled);
    }
    
    // Вспомогательный класс для мок-объекта в тесте Observer
    private class MockCourierObserver : StateControl.IOrderObserver
    {
        public bool UpdateCalled { get; private set; }
        public void Reset() => UpdateCalled = false;
        public void Update(Order.Order order)
        {
            UpdateCalled = true;
        }
    }
}
}