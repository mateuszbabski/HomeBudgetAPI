using HomeBudget.Models;
using HomeBudget.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeBudget.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        //get all
        [HttpGet]
        public ActionResult<IEnumerable<BudgetModel>> GetAll()
        {
            var budgets = _budgetService.GetAll();

            return Ok(budgets);

        }
        //get
        [HttpGet("{id}")]

        public ActionResult<BudgetModel> Get(int id)
        {
            var budget = _budgetService.GetById(id);

            return Ok(budget);
        }
        //create

        [HttpPost]

        public ActionResult AddBudget(CreateBudgetModel dto)
        {
            var id = _budgetService.Create(dto);

            return Created($"/api/budget/{id}", null);
        }
        //update
        [HttpPut("{id}")]
        public ActionResult Update(UpdateBudget dto, int id)
        {
            _budgetService.Update(id, dto);

            return Ok();
        }
        //delete
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _budgetService.Delete(id);

            return NoContent();
        }
    }
}
