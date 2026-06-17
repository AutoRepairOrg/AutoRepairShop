using AutoRepairShop.Domain.Exceptions;
using AutoRepairShop.Domain.ValueObjects;

namespace AutoRepairShop.Domain.Entities;

public class Vehicle
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public VehiclePlate Plate { get; private set; } = null!;
    public string Brand { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public int Year { get; private set; }

    protected Vehicle() { }

    public Vehicle(Guid customerId, VehiclePlate plate, string brand, string model, int year)
    {
        Id = Guid.NewGuid();

        SetCustomer(customerId);
        SetPlate(plate);
        SetBrand(brand);
        SetModel(model);
        SetYear(year);
    }

    private Vehicle(Guid id, Guid customerId, VehiclePlate plate, string brand, string model, int year)
    {
        Id = id;
        SetCustomer(customerId);
        SetPlate(plate);
        SetBrand(brand);
        SetModel(model);
        SetYear(year);
    }

    public static Vehicle Restore(Guid id, Guid customerId, VehiclePlate plate, string brand, string model, int year)
    {
        return new Vehicle(id, customerId, plate, brand, model, year);
    }

    public void Update(string brand, string model, int year)
    {
        SetBrand(brand);
        SetModel(model);
        SetYear(year);
    }

    private void SetCustomer(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("Customer is required.");

        CustomerId = customerId;
    }

    private void SetPlate(VehiclePlate plate)
    {
        Plate = plate ?? throw new DomainException("Vehicle plate is required.");
    }

    private void SetBrand(string brand)
    {
        if (string.IsNullOrWhiteSpace(brand))
            throw new DomainException("Brand is required.");

        if (brand.Length > 50)
            throw new DomainException("Brand cannot exceed 50 characters.");

        Brand = brand.Trim();
    }

    private void SetModel(string model)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new DomainException("Model is required.");

        if (model.Length > 50)
            throw new DomainException("Model cannot exceed 50 characters.");

        Model = model.Trim();
    }

    private void SetYear(int year)
    {
        var maxYear = DateTime.UtcNow.Year + 1;

        if (year < 1886 || year > maxYear)
            throw new DomainException("Invalid vehicle year.");

        Year = year;
    }
}
