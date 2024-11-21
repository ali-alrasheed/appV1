using Application.Domain.Madels;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveriesController : ControllerBase
    {
        private readonly IDeliveryRepository repository;

        public DeliveriesController(IDeliveryRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        public async Task<ActionResult> OrderDelivered( Guid OrderId , Guid InvoiceId)
        {
            await repository.OrderDelivered(OrderId, InvoiceId);
            return Ok();
        }

        [HttpGet("ByStatus")]
        public async Task<ActionResult<IList<Delivery>>> GetDeliveriesByStatus(DeliveryStatus status)
        {
            var deliveries = await repository.GetDeliveriesByStatusAsync(status);
            return Ok(deliveries);
        }

        [HttpPut("UpdateStatus")]
        public async Task<ActionResult> UpdateDeliveryStatus(Guid deliveryId, DeliveryStatus newStatus)
        {
            await repository.UpdateDeliveryStatus(deliveryId, newStatus);
            return Ok();
        }

        [HttpPut("AssignDriver")]
        public async Task<ActionResult> AssignDriver(Guid deliveryId, Guid driverId)
        {
            await repository.AssignDriverToDelivery(deliveryId, driverId);
            return Ok();
        }


        [HttpGet("Driver/{driverId}")]
        public async Task<ActionResult<IList<Delivery>>> GetDeliveriesByDriver(Guid driverId)
        {
            var deliveries = await repository.GetDeliveriesByDriverAsync(driverId);
            return Ok(deliveries);
        }

        [HttpDelete("Cancel/{deliveryId}")]
        public async Task<ActionResult> CancelDelivery(Guid deliveryId)
        {
            await repository.CancelDelivery(deliveryId);
            return Ok();
        }

    }
}
