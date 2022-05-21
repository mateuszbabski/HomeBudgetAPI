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

        public BudgetController(IBudgetService budgetService,
            IAuthorizationService authorizationService)
        {
            _budgetService = budgetService;
            _authorizationService = authorizationService;
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
            
            var budget = await _budgetService.GetById(id);
            return Ok(budget);
        }

        //create

        [HttpPost]

        public async Task<IActionResult> AddBudget(CreateBudgetModel dto)
        {
            var userId = int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var id = await _budgetService.Create(dto);

            return Created($"/api/budget/{id}", null);
        }
        //update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateBudget dto, int id)
        {
            
            await _budgetService.Update(id, dto);
            return Ok($"Budget {id} Updated");
        }

        //delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            await _budgetService.Delete(id);
            return NoContent();
        }
    }
}

          
