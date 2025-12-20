namespace DeliverySystem.MenuManager
{
public sealed class MenuManager
{
    private static MenuManager? _instance = null;
    private Dictionary<int, MenuItem.MenuItem> _menuItems;
    private MenuManager()
    {
        _menuItems = new Dictionary<int, MenuItem.MenuItem>();
    }

    public static MenuManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MenuManager();
            }
            return _instance;
        }
    }

    public MenuItem.MenuItem? GetItem(int id)
    {
        return _menuItems.TryGetValue(id, out var item) ? item : null;
    }

    public void AddMenuItem(MenuItem.MenuItem item)
{
    if (_menuItems.ContainsKey(item.Id))
    {
        throw new ArgumentException($"Блюдо с ID {item.Id} уже существует в меню.");
    }
    _menuItems.Add(item.Id, item);
}
}
}