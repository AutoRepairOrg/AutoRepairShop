using AutoRepairShop.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AutoRepairShop.Api.Controllers
{
    [Route("api/service-order")]
    [ApiController]
    public class ServiceOrderController : ControllerBase
    {
        private readonly IServiceOrderService _service;

        public ServiceOrderController(IServiceOrderService service)
        {
            _service = service;
        }

        //endpoint de criação de ordem de serviço
        //endpoint de consulta de tempo médio
    }
}
