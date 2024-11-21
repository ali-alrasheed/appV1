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
    public class StoresController : ControllerBase
    {
        private readonly int maxPageSize = 10;
        private readonly IStoreRepository _repository;
        private readonly ILogger<StoresController> _logger;

        public StoresController(IStoreRepository repository, ILogger<StoresController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Store>> Create(StoreForCreate store)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid store creation request.");
                    return BadRequest(ModelState);
                }

                var storeCreated = await _repository.AddAsync(store);
                if (storeCreated is null)
                {
                    _logger.LogWarning("Store creation failed.");
                    return BadRequest(new { message = "Failed to create the store." });
                }

                _logger.LogInformation("Store {StoreId} created successfully.", storeCreated.Id);
                return CreatedAtRoute("GetStore", new { storeId = storeCreated.Id }, storeCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a store.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the store." });
            }
        }

        [HttpGet("{storeId}", Name = "GetStore")]
        public async Task<ActionResult<Store>> GetStore(Guid storeId, bool isInclude = false)
        {
            try
            {
                var existingStore = await _repository.GetAsync(storeId, isInclude);
                if (existingStore is null)
                {
                    _logger.LogWarning("Store {StoreId} not found.", storeId);
                    return NotFound(new { message = "Store not found." });
                }

                _logger.LogInformation("Store {StoreId} retrieved successfully.", storeId);
                return Ok(existingStore);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving store {StoreId}.", storeId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the store." });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Store>>> GetAll(int pageNumber = 1, int pageSize = 10, bool isInclude = false)
        {
            try
            {
                if (pageSize > maxPageSize)
                    pageSize = maxPageSize;

                var (stores, paginationMetaData) = await _repository.GetAllAsync(pageNumber, pageSize, isInclude);

                if (stores == null || !stores.Any())
                {
                    _logger.LogWarning("No stores found.");
                    return NotFound(new { message = "No stores found." });
                }

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));
                _logger.LogInformation("Stores retrieved successfully with pagination.");
                return Ok(stores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving stores.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving stores." });
            }
        }

        [HttpDelete("{storeId}")]
        public async Task<ActionResult> Delete(Guid storeId)
        {
            try
            {
                var existingStore = await _repository.GetAsync(storeId);
                if (existingStore is null)
                {
                    _logger.LogWarning("Store {StoreId} not found.", storeId);
                    return NotFound(new { message = "Store not found." });
                }

                await _repository.DeleteAsync(storeId);
                _logger.LogInformation("Store {StoreId} deleted successfully.", storeId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting store {StoreId}.", storeId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while deleting the store." });
            }
        }
    }
}
