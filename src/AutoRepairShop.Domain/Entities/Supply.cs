public class Supply
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public decimal Price { get; private set; }

    public int StockQuantity { get; private set; }

    protected Supply() { }

    public Supply(string name, decimal price, int stockQuantity)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        StockQuantity = stockQuantity;
    }

    public void AddStock(int quantity)
    {
        StockQuantity += quantity;
    }

    public void RemoveStock(int quantity)
    {
        StockQuantity -= quantity;
    }

    public void Update(string name, decimal price)
    {
        Name = name;
        Price = price;
    }
}