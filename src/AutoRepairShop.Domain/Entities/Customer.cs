using AutoRepairShop.Domain.Exceptions;
using AutoRepairShop.Domain.Interfaces;
using AutoRepairShop.Domain.ValueObjects;

namespace AutoRepairShop.Domain.Entities;

public class Customer : IUser
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Document Document { get; private set; } = null!;
    public string Phone { get; private set; } = string.Empty;
    public string Username { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;

    public Customer() { }

    public Customer(string name, Document document, string phone, string username, string password)
    {
        Id = Guid.NewGuid();

        SetName(name);
        Document = document ?? throw new DomainException("Document is required");

        SetPhone(phone);
        SetUserName(username);
        Password = password;
    }

    private Customer(Guid id, string name, Document document, string phone, string username, string password)
    {
        Id = id;
        SetName(name);
        Document = document ?? throw new DomainException("Document is required");
        SetPhone(phone);
        SetUserName(username);
        Password = password;
    }

    public static Customer Restore(Guid id, string name, Document document, string phone, string username, string password)
    {
        return new Customer(id, name, document, phone, username, password);
    }

    public void Update(string name, string phone, string username, string password)
    {
        SetName(name);
        SetPhone(phone);
        SetUserName(username);
        Password = password;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer name is required");

        Name = name.Trim();
    }

    private void SetPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Phone is required");

        phone = phone.Trim();

        if (!phone.All(char.IsDigit))
            throw new DomainException("Phone must contain only numbers");

        if (phone.Length < 10 || phone.Length > 11)
            throw new DomainException("Phone must have 10 or 11 digits");

        Phone = phone;
    }

    private void SetUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new DomainException("UserName is required");

        Username = userName;
    }
}
