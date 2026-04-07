public class Customer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Document Document { get; private set; } // CPF or CNPJ
    public string Phone { get; private set; }
    public string Email { get; private set; }

    protected Customer() { }

    public Customer(string name, Document document, string phone, string email)
    {
        Id = Guid.NewGuid();
        Name = name;
        Document = document;
        Phone = phone;
        Email = email;
    }

    public void Update(string name, string phone, string email)
    {
        Name = name;
        Phone = phone;
        Email = email;
    }
}