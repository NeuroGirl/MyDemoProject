namespace DeliverySystem.OrderComponent
{
public abstract class OrderComponent
{
    public abstract string GetDescription();
    public abstract decimal GetPrice();
}

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

public class DishComposite : OrderComponent
{
    private string _name;
    private List<OrderComponent> _children = new List<OrderComponent>();

    public DishComposite(string name)
    {
        _name = name;
    }

    public void Add(OrderComponent component)
    {
        _children.Add(component);
    }

    public void Remove(OrderComponent component)
    {
        _children.Remove(component);
    }

    public override string GetDescription()
    {
        var childDescriptions = _children.Select(c => c.GetDescription());
        return $"Комбо '{_name}': ({string.Join(", ", childDescriptions)})";
    }

    public override decimal GetPrice()
    {
        return _children.Sum(c => c.GetPrice());
    }
}
}