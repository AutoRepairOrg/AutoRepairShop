using AutoRepairShop.Domain.Interfaces;

namespace AutoRepairShop.Domain.Entities
{
    public class Admin : IUser
    {
        public string Name { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public Admin() { }

        public Admin(string name, string department, string username, string passwordHash)
        {
            Id = Guid.NewGuid();
            Name = name;
            Department = department;
            Username = username;
            Password = passwordHash;
        }
    }
}
