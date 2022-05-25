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
        private readonly ILogger<BudgetController> _logger;
        private readonly MemoryCacheService _cacheService;
        
        public BudgetController(IBudgetService budgetService,
            ILogger<BudgetController> logger, 
            MemoryCacheService cacheService)
            
        {
            _budgetService = budgetService;
            _logger = logger;
            _cacheService = cacheService;
        }
             
            
        //get all
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BudgetModel>>> GetAll()
        {
            _logger.LogInformation("GetAll Budget controller method invoked");

            if(!_cacheService.TryGet(CacheKeys.Budgets, out IEnumerable<BudgetModel> budgets))
            {
                _logger.LogInformation("Budget list not found in cache");
                budgets = await _budgetService.GetAll();
                _cacheService.Set(CacheKeys.Budgets, budgets);
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
            _cacheService.Remove(CacheKeys.Budgets);
            return Ok(budget);
        }

        //create

        [HttpPost]

        public async Task<IActionResult> AddBudget(CreateBudgetModel dto)
        {
            _logger.LogInformation($"Add new budget controller method invoked");
            var userId = int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var id = await _budgetService.Create(dto);
            _cacheService.Remove(CacheKeys.Budgets);
            return Created($"/api/budget/{id}", null);
        }
        //update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateBudget dto, int id)
        {
            _logger.LogInformation($"Update budget({id}) controller method invoked");
            await _budgetService.Update(id, dto);
            _cacheService.Remove(CacheKeys.Budgets);
            return Ok($"Budget {id} Updated");
        }

        //delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Delete budget({id}) controller method invoked");
            await _budgetService.Delete(id);
            _cacheService.Remove(CacheKeys.Budgets);
            return NoContent();
        }
    }
}

          
        

            

