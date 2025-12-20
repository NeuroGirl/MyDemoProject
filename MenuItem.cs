namespace DeliverySystem.MenuItem
{
public class MenuItem
{
    public int Id { get; set;}
    public string Name { get; set;}
    public decimal BasePrice { get; set;}

    public MenuItem(int id, string name, decimal basePrice)
    {
        Id = id;
        Name = name;
        BasePrice = basePrice;
    }
}
}