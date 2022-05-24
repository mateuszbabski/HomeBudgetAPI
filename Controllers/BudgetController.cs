using HomeBudget.Cache;
using HomeBudget.Models;
using HomeBudget.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace HomeBudget.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<BudgetController> _logger;
        private readonly IMemoryCache _cache;

        public BudgetController(IBudgetService budgetService,
            IAuthorizationService authorizationService,
            ILogger<BudgetController> logger, 
            IMemoryCache cache)
        {
            _budgetService = budgetService;
            _authorizationService = authorizationService;
            _logger = logger;
            _cache = cache;
        }

        //get all
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BudgetModel>>> GetAll()
        {
            _logger.LogInformation("GetAll Budget controller method invoked");

            if(_cache.TryGetValue(CacheKeys.Budgets, out IEnumerable<BudgetModel> budgets))
            {
                _logger.LogInformation("Budget list found in cache");
            }
            else
            {
                _logger.LogInformation("Budget list not found in cache. Fetching from database");
                budgets = await _budgetService.GetAll();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(300))
                    .SetPriority(CacheItemPriority.Normal)
                    .SetSize(512);
                
                _cache.Set(CacheKeys.Budgets, budgets, cacheEntryOptions);
            }
            //var budgets = await _budgetService.GetAll();

            return Ok(budgets);
        }
            


        //get
        [HttpGet("{id}")]

        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation($"GetById({id}) budget controller method invoked");
            var budget = await _budgetService.GetById(id);
            _cache.Remove(CacheKeys.Budgets);
            return Ok(budget);
        }

        //create

        [HttpPost]

        public async Task<IActionResult> AddBudget(CreateBudgetModel dto)
        {
            _logger.LogInformation($"Add new budget controller method invoked");
            var userId = int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var id = await _budgetService.Create(dto);
            _cache.Remove(CacheKeys.Budgets);
            return Created($"/api/budget/{id}", null);
        }
        //update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateBudget dto, int id)
        {
            _logger.LogInformation($"Update budget({id}) controller method invoked");
            await _budgetService.Update(id, dto);
            _cache.Remove(CacheKeys.Budgets);
            return Ok($"Budget {id} Updated");
        }

        //delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Delete budget({id}) controller method invoked");
            await _budgetService.Delete(id);
            _cache.Remove(CacheKeys.Budgets);
            return NoContent();
        }
    }
}

          
