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
    public class CategoriesController : ControllerBase
    {
        private readonly int maxPageSize = 10;
        private readonly ICategoriesRepository _repository;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoriesRepository repository, ILogger<CategoriesController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Create(CategoryForCreate category)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var categoryCreated = await _repository.AddAsync(category);
                if (categoryCreated is null)
                    return BadRequest(new { message = "Failed to create the category." });

                _logger.LogInformation("Category {CategoryId} created successfully.", categoryCreated.Id);
                return CreatedAtRoute("GetCategory", new { categoryId = categoryCreated.Id }, categoryCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating category.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the category." });
            }
        }

        [HttpGet("{categoryId}", Name = "GetCategory")]
        public async Task<ActionResult> GetCategory(Guid categoryId, bool isInclude = false)
        {
            try
            {
                var existingCategory = await _repository.GetAsync(categoryId, isInclude);
                if (existingCategory is null)
                {
                    _logger.LogWarning("Category {CategoryId} not found.", categoryId);
                    return NotFound(new { message = "Category not found." });
                }

                _logger.LogInformation("Category {CategoryId} retrieved successfully.", categoryId);
                return Ok(existingCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving category {CategoryId}.", categoryId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the category." });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll(int pageNumber = 1, int pageSize = 10, bool isInclude = false)
        {
            try
            {
                if (pageSize > maxPageSize)
                    pageSize = maxPageSize;

                var (categories, paginationMetaData) = await _repository.GetAllAsync(pageNumber, pageSize, isInclude);

                if (categories == null || !categories.Any())
                {
                    _logger.LogWarning("No categories found.");
                    return NotFound(new { message = "No categories found." });
                }

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));
                _logger.LogInformation("Categories retrieved successfully with pagination.");
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving categories.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving categories." });
            }
        }

        [HttpDelete("{categoryId}")]
        public async Task<ActionResult> Delete(Guid categoryId)
        {
            try
            {
                var existingCategory = await _repository.GetAsync(categoryId);
                if (existingCategory is null)
                {
                    _logger.LogWarning("Category {CategoryId} not found.", categoryId);
                    return NotFound(new { message = "Category not found." });
                }

                await _repository.DeleteAsync(categoryId);
                _logger.LogInformation("Category {CategoryId} deleted successfully.", categoryId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category {CategoryId}.", categoryId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the category." });
            }
        }

    }
}
