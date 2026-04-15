namespace AutoRepairShop.Domain.Interfaces
{
    public interface IUser
    {
        Guid Id { get;}
        string Username { get;}
        string Password { get;}
    }
}
