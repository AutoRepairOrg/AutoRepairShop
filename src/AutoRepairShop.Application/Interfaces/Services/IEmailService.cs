using AutoRepairShop.Application.Interfaces.Messages;

namespace AutoRepairShop.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendAsync(IEmailMessage message);
}
