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
        public ActionResult<IEnumerable<BudgetModel>> GetAll()
        {
            
            var budgets = _budgetService.GetAll(User);

            return Ok(budgets);

        }
        //get
        [HttpGet("{id}")]

        public ActionResult<BudgetModel> Get(int id)
        {
            
            var budget = _budgetService.GetById(id, User);

            return Ok(budget);
        }
        //create

        [HttpPost]

        public ActionResult AddBudget(CreateBudgetModel dto)
        {
            var userId = int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var id = _budgetService.Create(dto, userId, User);

            return Created($"/api/budget/{id}", null);
        }
        //update
        [HttpPut("{id}")]
        public ActionResult Update(UpdateBudget dto, int id)
        {
            
            _budgetService.Update(id, dto, User);

            return Ok();
        }
        //delete
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {

            _budgetService.Delete(id, User);

            return NoContent();
        }
    }
}
