using HomeBudget.Models;
using HomeBudget.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public BudgetController(IBudgetService budgetService,
            IAuthorizationService authorizationService,
            ILogger<BudgetController> logger)
        {
            _budgetService = budgetService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        //get all
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BudgetModel>>> GetAll()
        {
            var budgets = await _budgetService.GetAll();
            
            return Ok(budgets);
        }


        //get
        [HttpGet("{id}")]

        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation($"GetById({id}) budget controller method invoked");
            var budget = await _budgetService.GetById(id);
            return Ok(budget);
        }

        //create

        [HttpPost]

        public async Task<IActionResult> AddBudget(CreateBudgetModel dto)
        {
            _logger.LogInformation($"Add new budget controller method invoked");
            var userId = int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var id = await _budgetService.Create(dto);

            return Created($"/api/budget/{id}", null);
        }
        //update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateBudget dto, int id)
        {
            _logger.LogInformation($"Update budget({id}) controller method invoked");
            await _budgetService.Update(id, dto);
            return Ok($"Budget {id} Updated");
        }

        //delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Delete budget({id}) controller method invoked");
            await _budgetService.Delete(id);
            return NoContent();
        }
    }
}

          
