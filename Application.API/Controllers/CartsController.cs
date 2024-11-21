using Application.Domain.Madels;
using Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Application.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartRepository _repository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CartsController> _logger;

        public CartsController(ICartRepository repository, UserManager<User> userManager, ILogger<CartsController> logger)
        {
            _repository = repository;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("{cartId}/{userId}/ConfirmOrder")]
        public async Task<ActionResult<Order>> ConfirmOrder(Guid cartId, Guid userId, int discount = 0)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                    return NotFound(new { message = "User not found" });
                }

                var order = await _repository.ConfirmOrder(cartId, userId, discount);
                if (order == null)
                {
                    _logger.LogWarning("Order confirmation failed for user {UserId} and cart {CartId}.", userId, cartId);
                    return BadRequest(new { message = "Order confirmation failed." });
                }

                _logger.LogInformation("Order confirmed for user {UserId}, cart {CartId}.", userId, cartId);
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming order for user {UserId}, cart {CartId}.", userId, cartId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while confirming the order." });
            }
        }
        [HttpPost("AddItem")]
        public async Task<ActionResult<Cart>> AddItemToCart(Guid? cartId, Guid userId, Guid productId, int quantity = 1)
        {
            try
            {
                var cart = await _repository.CreateOrUpdate(cartId, userId, productId, quantity);
                if (cart == null)
                {
                    _logger.LogWarning("Failed to add item {ProductId} to cart for user {UserId}.", productId, userId);
                    return BadRequest(new { message = "Failed to add item to cart." });
                }

                _logger.LogInformation("Item {ProductId} added to cart {CartId} for user {UserId}.", productId, cart?.Id, userId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item {ProductId} to cart for user {UserId}.", productId, userId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while adding the item to the cart." });
            }
        }

        [HttpGet("{cartId}")]
        public ActionResult GetCartDetails(Guid cartId)
        {
            try
            {
                var cart = _repository.GetCartWithDetails(cartId);
                if (cart == null)
                {
                    _logger.LogWarning("Cart with ID {CartId} not found.", cartId);
                    return NotFound(new { message = "Cart not found" });
                }

                _logger.LogInformation("Cart {CartId} retrieved successfully.", cartId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart details for cart {CartId}.", cartId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the cart details." });
            }
        }

        [HttpDelete("RemoveItem/{cartId}/{cartItemId}")]
        public async Task<ActionResult<Cart>> RemoveItemFromCart(Guid cartId, Guid cartItemId)
        {
            try
            {
                var cart = await _repository.GetCartWithDetails(cartId);
                var item = cart.CartItems.FirstOrDefault(x => x.Id == cartItemId);
                if (item == null)
                {
                    _logger.LogWarning("Item {CartItemId} not found in cart {CartId}.", cartItemId, cartId);
                    return NotFound(new { message = "Item not found in cart" });
                }

                _repository.DeleteCartItem(cartItemId);
                _logger.LogInformation("Item {CartItemId} removed from cart {CartId}.", cartItemId, cartId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item {CartItemId} from cart {CartId}.", cartItemId, cartId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while removing the item from the cart." });
            }
        }

        [HttpDelete("ClearCart/{cartId}")]
        public async Task<ActionResult<Cart>> ClearCart(Guid cartId)
        {
            try
            {
                var cart = await _repository.GetCartWithDetails(cartId);
                if (cart == null)
                {
                    _logger.LogWarning("Cart {CartId} not found.", cartId);
                    return NotFound(new { message = "Cart not found" });
                }

                cart.CartItems.Clear();
                _repository.Update(cart);
                _logger.LogInformation("Cart {CartId} cleared successfully.", cartId);
                return Ok(new { message = "Cart cleared successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart {CartId}.", cartId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while clearing the cart." });
            }
        }
    }
}
