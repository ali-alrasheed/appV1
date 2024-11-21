using Application.Domain.Madels;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.ViewModels.Creation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Application.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly int maxPageSize = 10;
        private readonly IOrderRepository _repository;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderRepository repository, ILogger<OrdersController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> Create(OrderForCreate order)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var orderCreated = await _repository.AddAsync(order);
                if (orderCreated is null)
                {
                    _logger.LogWarning("Failed to create order. Order data might be invalid.");
                    return BadRequest(new { message = "Failed to create the order." });
                }

                _logger.LogInformation("Order {OrderId} created successfully.", orderCreated.Id);
                return CreatedAtRoute("GetOrder", new { orderId = orderCreated.Id }, orderCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating an order.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the order." });
            }
        }

        [HttpGet("{orderId}", Name = "GetOrder")]
        public async Task<ActionResult<Order>> GetOrder(Guid orderId, bool isInclude = false)
        {
            try
            {
                var existingOrder = await _repository.GetAsync(orderId, isInclude);
                if (existingOrder is null)
                {
                    _logger.LogWarning("Order {OrderId} not found.", orderId);
                    return NotFound(new { message = "Order not found." });
                }

                _logger.LogInformation("Order {OrderId} retrieved successfully.", orderId);
                return Ok(existingOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving order {OrderId}.", orderId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the order." });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAll(int pageNumber = 1, int pageSize = 10, bool isInclude = false)
        {
            try
            {
                if (pageSize > maxPageSize)
                    pageSize = maxPageSize;

                var (orders, paginationMetaData) = await _repository.GetAllAsync(pageNumber, pageSize, isInclude);

                if (orders == null || !orders.Any())
                {
                    _logger.LogWarning("No orders found.");
                    return NotFound(new { message = "No orders found." });
                }

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));
                _logger.LogInformation("Orders retrieved successfully with pagination.");
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving orders." });
            }
        }

        [HttpDelete("{orderId}")]
        public async Task<ActionResult> Delete(Guid orderId)
        {
            try
            {
                var existingOrder = await _repository.GetAsync(orderId);
                if (existingOrder is null)
                {
                    _logger.LogWarning("Order {OrderId} not found.", orderId);
                    return NotFound(new { message = "Order not found." });
                }

                await _repository.DeleteAsync(orderId);
                _logger.LogInformation("Order {OrderId} deleted successfully.", orderId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting order {OrderId}.", orderId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the order." });
            }
        }
    }
}
