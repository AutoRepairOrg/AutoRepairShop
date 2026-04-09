public class Vehicle
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public VehiclePlate Plate { get; private set; }
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public int Year { get; private set; }

    protected Vehicle() { }

    public Vehicle(Guid customerId, VehiclePlate plate, string brand, string model, int year)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Plate = plate;
        Brand = brand;
        Model = model;
        Year = year;
    }

    public void Update(string brand, string model, int year)
    {
        Brand = brand;
        Model = model;
        Year = year;
    }
}