using AutoRepairShop.Domain.Exceptions;
using AutoRepairShop.Domain.Interfaces;

namespace AutoRepairShop.Domain.Entities;
public class Customer : IUser
{
    public string Name { get; private set; }
    public Document Document { get; private set; }
    public string Phone { get; private set; }
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string Password { get; private set; }

    protected Customer() { } 

    public Customer(
        string name,
        Document document,
        string phone,
        string username,
        string password)
    {
        Id = Guid.NewGuid();

        ChangeName(name);
        Document = document
            ?? throw new DomainException("Document is required");

        ChangePhone(phone);
        ChangeUserName(username);
        Password = password;
    }

    public void Update(string name, string phone, string username, string password)
    {
        ChangeName(name);
        ChangePhone(phone);
        ChangeUserName(username);
        Password = password;
    }

    private void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer name is required");

        Name = name.Trim();
    }
    private void ChangePhone(string phone)
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

    private void ChangeUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new DomainException("UserName is required");

        Username = userName;

        //TODO: Validar quantidade de caracteres, validação de senha 
    }
}