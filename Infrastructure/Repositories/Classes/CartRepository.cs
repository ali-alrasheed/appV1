using Application.Domain.Madels;
using Infrastructure.DbContexts;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.ViewModels.Creation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Classes
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly AppDbContext context;
        private readonly IOrderRepository orderRepository;
        private readonly ILogger<CartRepository> _logger;

        public CartRepository(
            AppDbContext context,
            IOrderRepository orderRepository,
            ILogger<CartRepository> logger) : base(context)
        {
            this.context = context;
            this.orderRepository = orderRepository;
            this._logger = logger;
        }

        public async Task<Order> ConfirmOrder(Guid cartId, Guid userId, int discount = 0)
        {
            try
            {
                var cart = await GetCartWithDetails(cartId);
                if (cart == null || cart.UserId != userId)
                {
                    _logger.LogWarning("Cart with ID {CartId} not found or does not belong to user {UserId}.", cartId, userId);
                    return null;
                }

                var orderModel = new OrderForCreate
                {
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    TotalPrice = cart.TotalPrice - (cart.TotalPrice * discount / 100),
                    UserId = cart.UserId,
                    Items = cart.CartItems.Select(ci => new OrderItemForCreate
                    {
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                    }).ToList()
                };

                var order = await orderRepository.AddAsync(orderModel);
                if (order == null)
                {
                    _logger.LogError("Failed to create order for cart {CartId}.", cartId);
                    return null;
                }

                cart.CartItems.Clear(); // Clear cart items after creating the order
                await context.SaveChangesAsync();
                _logger.LogInformation("Order {OrderId} created successfully for cart {CartId}.", order.Id, cartId);

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming order for cart {CartId}.", cartId);
                throw;
            }
        }

        public async Task<Cart> CreateOrUpdate(Guid? cartId, Guid userId, Guid productId, int quantity = 1)
        {
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
                if (user is null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                    return null;
                }

                Cart cart;
                if (cartId is null)
                {
                    cart = new Cart
                    {
                        DateCreated = DateTime.Now,
                        CartItems = new List<CartItem>(),
                        UserId = userId,
                        IsActive = true,
                    };
                    await context.Carts.AddAsync(cart);
                }
                else
                {
                    cart = await GetCartWithDetails(cartId.Value);
                    if (cart is null)
                    {
                        _logger.LogWarning("Cart with ID {CartId} not found.", cartId);
                        return null;
                    }
                }

                var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (existingCartItem is not null)
                {
                    existingCartItem.IsActive = true;
                    existingCartItem.Quantity += quantity;
                    context.Entry(existingCartItem).State = EntityState.Modified;
                }
                else
                {
                    await AddCartItemAsync(cart, productId, quantity);
                }

                await context.SaveChangesAsync();
                _logger.LogInformation("Cart {CartId} updated successfully.", cart.Id);

                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating or updating cart {CartId}.", cartId);
                throw;
            }
        }

        public async Task DeleteCartItem(Guid cartItemId)
        {
            try
            {
                var cartItem = await context.CartItems.FirstOrDefaultAsync(ci => ci.Id == cartItemId);
                if (cartItem is not null)
                {
                    cartItem.IsActive = false;
                    context.Update(cartItem);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("Cart item {CartItemId} deactivated successfully.", cartItemId);
                }
                else
                {
                    _logger.LogWarning("Cart item {CartItemId} not found.", cartItemId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cart item {CartItemId}.", cartItemId);
                throw;
            }
        }
       
        public async Task<Cart> GetCartWithDetails(Guid cartId)
        {
            return await context.Carts
                   .Include(c => c.CartItems.Where(ci => ci.IsActive))
                   .FirstOrDefaultAsync(c => c.Id == cartId && c.IsActive);
        }

        private async Task AddCartItemAsync(Cart cart, Guid productId, int quantity)
        {
            var product = await context.Products
                              .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);
            if (product is null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found or inactive.", productId);
                throw new Exception("Product not found or inactive.");
            }
            if (product.Quantity < quantity)
            {
                _logger.LogWarning("Product with ID {ProductId} not enough.", productId);
                throw new Exception("Product not enough .");
            }

            var newCartItem = new CartItem
            {
                IsActive = true,
                CartId = cart.Id,
                ProductId = productId,
                Price = product.Price,
                Quantity = quantity,
            };

            cart.CartItems.Add(newCartItem);
            context.Entry(newCartItem).State = EntityState.Added;
        }
    }
}
