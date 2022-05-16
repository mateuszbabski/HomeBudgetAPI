using AutoMapper;
using HomeBudget.Authorization;
using HomeBudget.Entities;
using HomeBudget.Exceptions;
using HomeBudget.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HomeBudget.Services
{
    public interface ITransactionService
    {
        int Create(CreateTransactionModel dto, int budgetId);
        void Delete(int id, int budgetId);
        IEnumerable<TransactionModel> GetAll(int budgetId, RequestParams request);
        //IEnumerable<TransactionModel> GetAll(int budgetId);

        TransactionModel GetById(int id, int budgetId);
        void Update(UpdateTransactionModel dto, int id, int budgetId);
        Budget GetBudgetById(int id);

        

    }



    public class TransactionService : ITransactionService
    {
        private readonly BudgetDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public TransactionService(BudgetDbContext dbContext, IMapper mapper, IAuthorizationService authorizationService,
            IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }

        public IEnumerable<TransactionModel> GetAll(int budgetId, RequestParams request)
        {
            var budget = GetBudgetById(budgetId);

            var transactions = _dbContext.Transactions
                .Where(x => x.BudgetID == budgetId)
                .OrderByDescending(x => x.TransactionDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            
            var transactionsDto = _mapper.Map<List<TransactionModel>>(transactions);


            return transactionsDto;
        }

        public TransactionModel GetById(int id, int budgetId)
        {
            var budget = GetBudgetById(budgetId);

            var transaction = _dbContext.Transactions
                .FirstOrDefault(x => x.Id == id);

            if(transaction is null || transaction.BudgetID != budgetId)
            {
                throw new NotFoundException("Transaction not found");
            }

            var result = _mapper.Map<TransactionModel>(transaction);

            return result;
        }




        //public IEnumerable<TransactionModel> GetAll(int budgetId)
        //{
        //    var budget = GetBudgetById(budgetId);

        //    var transactions = _dbContext.Transactions
        //        .Where(x => x.BudgetID == budgetId)
        //        .OrderByDescending(x => x.TransactionDate)
        //        .ToList();

        //    
        //    var transactionsDto = _mapper.Map<List<TransactionModel>>(transactions);


        //    return transactionsDto;
        //}

        public int Create(CreateTransactionModel dto, int budgetId)
        {
            var budget = GetBudgetById(budgetId);
            var transaction = _mapper.Map<Transaction>(dto);

            transaction.BudgetID = budgetId;
            

            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return transaction.Id;
        }

        public void Update(UpdateTransactionModel dto, int id, int budgetId)
        {
            var budget = GetBudgetById(budgetId);
            var transaction = _dbContext.Transactions
                .FirstOrDefault(x => x.Id == id);

            if (transaction is null || transaction.BudgetID != budgetId)
            {
                throw new NotFoundException("Transaction not found");
            }

            transaction.BudgetID = dto.BudgetID;
            transaction.Type = dto.Type;
            transaction.Category = dto.Category;
            transaction.Description = dto.Description;
            transaction.Value = dto.Value; 
            transaction.TransactionDate = dto.TransactionDate;

            _dbContext.SaveChanges();
        }

        public void Delete(int id, int budgetId)
        {
            var budget = GetBudgetById(budgetId);

            var transaction = _dbContext.Transactions
                .FirstOrDefault(x => x.Id == id);

            if (transaction is null || transaction.BudgetID != budgetId)
            {
                throw new NotFoundException("Transaction not found");
            }

            _dbContext.Transactions.Remove(transaction);
            _dbContext.SaveChanges();
        }

        public Budget GetBudgetById(int budgetId)
        {
            var userId = (int)_userContextService.GetUserId;

            var budget = _dbContext.Budgets
                .Where(b => b.UserID == userId)
                .Include(t => t.Transactions)
                .FirstOrDefault(m => m.Id == budgetId);

            if (budget is null)
            {
                throw new NotFoundException("Budget not found");
            }

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User,
                budget,
                new ResourceOperationRequirement(ResourceOperation.Read)).Result;


            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException("Forbidden - Authorization error");
            }

            var result = _mapper.Map<Budget>(budget);
            return result;
        }
    }
}


       

            
                
            



