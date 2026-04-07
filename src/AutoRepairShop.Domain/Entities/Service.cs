public class Service
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public decimal Price { get; private set; }

    public int EstimatedTimeInMinutes { get; private set; }

    protected Service() { }

    public Service(string name, string description, decimal price, int estimatedTimeInMinutes)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Price = price;
        EstimatedTimeInMinutes = estimatedTimeInMinutes;
    }

    public void Update(string name, string description, decimal price, int estimatedTimeInMinutes)
    {
        Name = name;
        Description = description;
        Price = price;
        EstimatedTimeInMinutes = estimatedTimeInMinutes;
    }
}