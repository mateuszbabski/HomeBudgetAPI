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
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ITransactionService transactionService,
            ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionModel>>> GetAll(int budgetId, [FromQuery] RequestParams request)
        {
            _logger.LogInformation($"Get all transactions from budget({budgetId}) controller method invoked");
            var transactions = await _transactionService.GetAll(budgetId, request);

            return Ok(transactions);
        }



        


        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionModel>> Get(int id, int budgetId)
        {
            _logger.LogInformation($"Get transaction({id} from budget({budgetId}) controller method invoked");
            var transaction = await _transactionService.GetById(id, budgetId);

            return Ok(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionModel dto, int budgetId)
        {
            _logger.LogInformation($"Create new transaction in budget({budgetId}) controller method invoked");
            var userId = int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var id = await _transactionService.Create(dto, budgetId);

            return Created($"/api/budget/{budgetId}/transaction/{id}", null);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(UpdateTransactionModel dto, int id, int budgetId)
        {
            _logger.LogInformation($"Update transaction({id} from budget({budgetId}) controller method invoked");
            await _transactionService.Update(dto, id, budgetId);

            return Ok($"Transaction {id} updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, int budgetId)
        {
            _logger.LogInformation($"Delete transaction({id} from budget({budgetId}) controller method invoked");
            await _transactionService.Delete(id, budgetId);

            return NoContent();
        }

    }
}
