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
    public class ProductsController : ControllerBase
    {
        private readonly int maxPageSize = 10;
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductRepository repository, ILogger<ProductsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create(ProductForCreate product)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var productCreated = await _repository.AddAsync(product);
                if (productCreated is null)
                {
                    _logger.LogWarning("Failed to create product. Product data might be invalid.");
                    return BadRequest(new { message = "Failed to create the product." });
                }

                _logger.LogInformation("Product {ProductId} created successfully.", productCreated.Id);
                return CreatedAtRoute("GetProduct", new { productId = productCreated.Id }, productCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a product.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the product." });
            }
        }

        [HttpGet("{productId}", Name = "GetProduct")]
        public async Task<ActionResult<Product>> GetProduct(Guid productId, bool isInclude = false)
        {
            try
            {
                var existingProduct = await _repository.GetAsync(productId, isInclude);
                if (existingProduct is null)
                {
                    _logger.LogWarning("Product {ProductId} not found.", productId);
                    return NotFound(new { message = "Product not found." });
                }

                _logger.LogInformation("Product {ProductId} retrieved successfully.", productId);
                return Ok(existingProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving product {ProductId}.", productId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the product." });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll(int pageNumber = 1, int pageSize = 10, bool isInclude = false)
        {
            try
            {
                if (pageSize > maxPageSize)
                    pageSize = maxPageSize;

                var (products, paginationMetaData) = await _repository.GetAllAsync(pageNumber, pageSize, isInclude);

                if (products == null || !products.Any())
                {
                    _logger.LogWarning("No products found.");
                    return NotFound(new { message = "No products found." });
                }

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));
                _logger.LogInformation("Products retrieved successfully with pagination.");
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving products." });
            }
        }

        [HttpDelete("{productId}")]
        public async Task<ActionResult> Delete(Guid productId)
        {
            try
            {
                var existingProduct = await _repository.GetAsync(productId);
                if (existingProduct is null)
                {
                    _logger.LogWarning("Product {ProductId} not found.", productId);
                    return NotFound(new { message = "Product not found." });
                }

                await _repository.DeleteAsync(productId);
                _logger.LogInformation("Product {ProductId} deleted successfully.", productId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product {ProductId}.", productId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the product." });
            }
        }
    }
}
