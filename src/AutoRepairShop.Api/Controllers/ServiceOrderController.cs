using AutoRepairShop.Application.DTOs.ServiceOrder.Request;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoRepairShop.Api.Controllers
{
    [Route("api/service-order")]
    [ApiController]
    public class ServiceOrderController(IServiceOrderService service) : ControllerBase
    {
        private readonly IServiceOrderService _service = service;

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateServiceOrderRequest request)
        {
            try
            {
                await _service.CreateServiceOrderAsync(request);
                return Ok();
            }
            catch (DomainException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        //endpoint de consulta de tempo médio
    }
}
