using AutoRepairShop.Domain.Interfaces;

namespace AutoRepairShop.Domain.Entities
{
    public class Admin : IUser
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Department { get; private set; } = string.Empty;
        public string Username { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;

        public Admin() { }

        public Admin(string name, string department, string username, string passwordHash)
        {
            Id = Guid.NewGuid();
            Name = name;
            Department = department;
            Username = username;
            Password = passwordHash;
        }

        private Admin(Guid id, string name, string department, string username, string passwordHash)
        {
            Id = id;
            Name = name;
            Department = department;
            Username = username;
            Password = passwordHash;
        }

        public static Admin Restore(Guid id, string name, string department, string username, string passwordHash)
        {
            return new Admin(id, name, department, username, passwordHash);
        }
    }
}
