public class Service
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }

    protected Service() { }

    public Service(string name, string description, decimal price)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Price = price;
    }

    public void Update(string name, string description, decimal price)
    {
        Name = name;
        Description = description;
        Price = price;
    }
}