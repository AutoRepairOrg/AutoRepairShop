using AutoRepairShop.Domain.Exceptions;
using AutoRepairShop.Domain.Interfaces;
using AutoRepairShop.Domain.ValueObjects;

namespace AutoRepairShop.Domain.Entities;

public class Customer : IUser
{
    public string Name { get; set; }
    public Document Document { get; set; }
    public string Phone { get; set; }
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

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

        //TODO: Validar quantidade de caracteres, validação de senha
    }
}
