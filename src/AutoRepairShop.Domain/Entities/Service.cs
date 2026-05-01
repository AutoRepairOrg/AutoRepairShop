using AutoRepairShop.Domain.Exceptions;

namespace AutoRepairShop.Domain.Entities;
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

        SetName(name);
        SetDescription(description);
        SetPrice(price);
    }

    public void Update(string name, string description, decimal price)
    {
        SetName(name);
        SetDescription(description);
        SetPrice(price);
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Service name is required.");

        if (name.Length > 100)
            throw new DomainException("Service name cannot exceed 100 characters.");

        Name = name.Trim();
    }

    private void SetDescription(string description)
    {
        if (!string.IsNullOrWhiteSpace(description) && description.Length > 500)
            throw new DomainException("Service description cannot exceed 500 characters.");

        Description = description?.Trim();
    }

    private void SetPrice(decimal price)
    {
        if (price <= 0)
            throw new DomainException("Service price must be greater than zero.");

        Price = price;
    }
}