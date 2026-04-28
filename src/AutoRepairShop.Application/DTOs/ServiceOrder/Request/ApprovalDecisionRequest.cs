namespace AutoRepairShop.Application.DTOs.ServiceOrder.Request
{
    public class ApprovalDecisionRequest
    {
        public Guid ServiceOrderId { get; set; }
        public bool IsApproved { get; set; }
    }
}
