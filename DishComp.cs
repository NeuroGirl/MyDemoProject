namespace DeliverySystem.OrderComponent
{
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