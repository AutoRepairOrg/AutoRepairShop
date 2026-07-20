using AutoRepairShop.Application.DTOs.ServiceOrder;
using AutoRepairShop.Application.Interfaces.Messages;
using AutoRepairShop.Domain.Enums;
using System.Text;
using CustomerEntity = AutoRepairShop.Domain.Entities.Customer;
using ServiceOrderEntity = AutoRepairShop.Domain.Entities.ServiceOrder.ServiceOrder;

namespace AutoRepairShop.Application.DTOs.Email.Messages;

public class ServiceOrderStatusChangedEmailMessage : IEmailMessage
{
    public string To { get; }
    public string Subject { get; }
    public string Body { get; }

    public ServiceOrderStatusChangedEmailMessage(
        CustomerEntity customer,
        ServiceOrderEntity serviceOrder,
        ServiceOrderSummary? summary = null
    )
    {
        To = customer.Email;
        Subject = BuildSubject(serviceOrder.Status);
        Body = BuildBody(customer, serviceOrder, summary);
    }

    private static string BuildSubject(ServiceOrderStatus status)
    {
        return status switch
        {
            ServiceOrderStatus.Received => "Ordem de serviço recebida",
            ServiceOrderStatus.InDiagnosis => "Ordem de serviço em diagnóstico",
            ServiceOrderStatus.WaitingApproval => "Ordem de serviço aguardando aprovação",
            ServiceOrderStatus.InExecution => "Ordem de serviço em execução",
            ServiceOrderStatus.Finished => "Ordem de serviço finalizada",
            ServiceOrderStatus.Delivered => "Ordem de serviço entregue",
            ServiceOrderStatus.Canceled => "Ordem de serviço cancelada",
            _ => "Atualização da ordem de serviço",
        };
    }

    private static string BuildBody(
        CustomerEntity customer,
        ServiceOrderEntity serviceOrder,
        ServiceOrderSummary? summary
    )
    {
        var body = new StringBuilder();
        body.AppendLine($"Olá, {customer.Name}.");
        body.AppendLine();
        body.AppendLine($"Sua ordem de serviço {serviceOrder.Id} foi atualizada.");
        body.AppendLine($"Status atual: {serviceOrder.Status}.");

        if (summary is not null)
        {
            body.AppendLine();
            body.AppendLine("Resumo do orçamento:");

            foreach (var service in summary.Services)
            {
                body.AppendLine($"- Serviço: {service.Name} | R$ {service.Price:F2}");
            }

            foreach (var supply in summary.Supplies)
            {
                body.AppendLine(
                    $"- Insumo: {supply.Name} | Qtd: {supply.Quantity} | R$ {supply.Price * supply.Quantity:F2}"
                );
            }

            body.AppendLine($"Total: R$ {summary.TotalAmount:F2}");
        }

        return body.ToString();
    }
}
