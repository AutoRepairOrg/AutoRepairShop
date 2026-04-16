using AutoRepairShop.Application.DTOs.Vehicle;
using AutoRepairShop.Application.Interfaces.Services;
using AutoRepairShop.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoRepairShop.Api.Controllers
{
    [Route("api/vehicle")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService VehicleService)
        {
            _vehicleService = VehicleService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<VehicleResponse>>> GetAll()
        {
            var result = await _vehicleService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<ActionResult<VehicleResponse>> GetById([FromRoute] Guid id)
        {
            var result = await _vehicleService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<ActionResult<VehicleResponse>> Delete([FromRoute] Guid id)
        {
            await _vehicleService.DeleteAsync(id);
            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> Create([FromBody] CreateVehicleRequest request)
        {
            try
            {
                var result = await _vehicleService.CreateAsync(request);
                return Ok(result);
            }
            catch (DomainException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateVehicleRequest request)
        {
            try
            {
                var result = await _vehicleService.UpdateAsync(request);
                return Ok(result);
            }
            catch (DomainException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
