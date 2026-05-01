using AutoRepairShop.Domain.Exceptions;

namespace AutoRepairShop.Domain.Entities;
public class Supply
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }

    protected Supply() { }

    public Supply(string name, decimal price, int stockQuantity)
    {
        Validate(name, price, stockQuantity);

        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        StockQuantity = stockQuantity;
    }

    public void Update(string name, decimal price, int stockQuantity)
    {
        Validate(name, price, stockQuantity);

        Name = name;
        Price = price;
        StockQuantity = stockQuantity;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (quantity > StockQuantity)
            throw new DomainException("Insufficient stock quantity.");

        StockQuantity -= quantity;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        StockQuantity += quantity;
    }

    private static void Validate(string name, decimal price, int stockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Supply name is required.");

        if (price <= 0)
            throw new DomainException("Price must be greater than zero.");

        if (stockQuantity < 0)
            throw new DomainException("Stock quantity cannot be negative.");
    }
}
