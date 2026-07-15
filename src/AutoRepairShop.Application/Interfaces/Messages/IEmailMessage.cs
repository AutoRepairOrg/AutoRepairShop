namespace AutoRepairShop.Application.Interfaces.Messages;

public interface IEmailMessage
{
    string To { get; }
    string Subject { get; }
    string Body { get; }
}
