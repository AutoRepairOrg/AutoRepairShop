using AutoRepairShop.Domain.Interfaces;

namespace AutoRepairShop.Domain.Entities
{
    public class Admin : IUser
    {
        public string Name { get; private set; }
        public string Department { get; private set; }
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        protected Admin() { }

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
