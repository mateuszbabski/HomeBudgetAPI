using HomeBudget.Models;
using HomeBudget.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeBudget.Controllers
{
    [Route("api/budget/{budgetId}/transactions/")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }



        [HttpGet]
        public ActionResult<IEnumerable<TransactionModel>> GetAll(int budgetId, [FromQuery] RequestParams request)
        {
            var transactions = _transactionService.GetAll(budgetId, request);

            return Ok(transactions);
        }



        


        [HttpGet("{id}")]
        public ActionResult<TransactionModel> Get(int id, int budgetId)
        {
            var transaction = _transactionService.GetById(id, budgetId);

            return Ok(transaction);
        }

        [HttpPost]
        public ActionResult Create(CreateTransactionModel dto, int budgetId)
        {
            var userId = int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var id = _transactionService.Create(dto, budgetId);

            return Created($"/api/budget/{budgetId}/transaction/{id}", null);
        }

        [HttpPut("{id}")]
        public ActionResult Update(UpdateTransactionModel dto, int id, int budgetId)
        {
            _transactionService.Update(dto, id, budgetId);

            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id, int budgetId)
        {
            _transactionService.Delete(id, budgetId);

            return NoContent();
        }

    }
}
