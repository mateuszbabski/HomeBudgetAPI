﻿using HomeBudget.Models;
using HomeBudget.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeBudget.Controllers
{
    [Route("api/budget/{budgetid}/transaction/")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
     
        [HttpGet]
        public ActionResult<IEnumerable<TransactionModel>> GetAll(int budgetId)
        {
            var transactions = _transactionService.GetAll(budgetId);

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