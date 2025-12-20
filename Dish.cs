namespace DeliverySystem.OrderComponent
{
public class Dish : OrderComponent
{
    private MenuItem.MenuItem _item;

    public Dish(MenuItem.MenuItem item)
    {
        _item = item;
        
    }
    
    public override string GetDescription() => _item.Name;
    public override decimal GetPrice() => _item.BasePrice;
}
}