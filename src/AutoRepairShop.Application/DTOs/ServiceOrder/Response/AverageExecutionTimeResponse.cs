namespace AutoRepairShop.Application.DTOs.ServiceOrder.Response
{
    public class AverageExecutionTimeResponse
    {
        public int TotalServiceOrders { get; set; }
        public int CompletedServiceOrders { get; set; }
        public double AverageExecutionTimeInHours { get; set; }
        public double AverageExecutionTimeInDays { get; set; }
        public DateTime? EarliestStartDate { get; set; }
        public DateTime? LatestFinishDate { get; set; }
    }
}
