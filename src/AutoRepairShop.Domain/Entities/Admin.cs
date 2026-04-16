using AutoRepairShop.Domain.Interfaces;

namespace AutoRepairShop.Domain.Entities
{
    public class Admin : IUser
    {
        public string Name { get; set; }
        public string Department { get; set; }
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

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
