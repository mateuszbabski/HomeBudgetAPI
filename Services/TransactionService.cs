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
        Task<int> Create(CreateTransactionModel dto, int budgetId);
        Task<Transaction> Delete(int id, int budgetId);
        Task<PagedList<TransactionModel>> GetAll(int budgetId, RequestParams request);
        Task<TransactionModel> GetById(int id, int budgetId);
        Task<int> Update(UpdateTransactionModel dto, int id, int budgetId);
        Task<Budget> GetBudgetById(int budgetId);
    }

    public class TransactionService : ITransactionService
    {
        private readonly BudgetDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(BudgetDbContext dbContext,
            IMapper mapper,
            IAuthorizationService authorizationService,
            IUserContextService userContextService, 
            ILogger<TransactionService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
            _logger = logger;
        }
        

        public async Task<PagedList<TransactionModel>> GetAll(int budgetId, RequestParams request)
        {
            
            var budget = await GetBudgetById(budgetId);

            var baseQuery = _dbContext.Transactions
                .Where(x => x.BudgetID == budgetId)
                .Where(x => request.SearchPhrase == null || (x.Category.ToLower().Contains(request.SearchPhrase.ToLower())
                                                         || x.Type.ToLower().Contains(request.SearchPhrase.ToLower())))
                .OrderByDescending(x => x.TransactionDate);




            if (!string.IsNullOrEmpty(request.SortBy))
            {
                var columnsSelector = new Dictionary<string, Expression<Func<Transaction, object>>>
                {
                    { nameof(Transaction.Value), r => r.Value },
                    { nameof(Transaction.TransactionDate), r => r.TransactionDate },
                    { nameof(Transaction.Type), r => r.Type },
                    
                };

                var selectedColumn = columnsSelector[request.SortBy];

                baseQuery = request.SortDirection == SortDirection.ASC
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var transactions = await baseQuery
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
                

            var totalCount = baseQuery.Count();

            var transactionsDto = _mapper.Map<List<TransactionModel>>(transactions);

            var result = new PagedList<TransactionModel>(transactionsDto, totalCount, request.PageNumber, request.PageSize);
            

            return result;
        }


        public async Task<TransactionModel> GetById(int id, int budgetId)
        {
            
            var budget = await GetBudgetById(budgetId);

            var transaction = await _dbContext.Transactions
                .FirstOrDefaultAsync(x => x.Id == id);

            if(transaction is null || transaction.BudgetID != budgetId)
            {
                _logger.LogError("Transaction not found");
                throw new NotFoundException("Transaction not found");
            }

            var result = _mapper.Map<TransactionModel>(transaction);

            return result;
        }

        public async Task<int> Create(CreateTransactionModel dto, int budgetId)
        {
            
            var budget = await GetBudgetById(budgetId);
            var transaction = _mapper.Map<Transaction>(dto);

            transaction.BudgetID = budgetId;
            

            await _dbContext.Transactions.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();

            return transaction.Id;
        }

        public async Task<int> Update(UpdateTransactionModel dto, int id, int budgetId)
        {
            
            var budget = await GetBudgetById(budgetId);
            var transaction = await _dbContext.Transactions
                .FirstOrDefaultAsync(x => x.Id == id);

            if (transaction is null || transaction.BudgetID != budgetId)
            {
                _logger.LogError("Transaction not found");
                throw new NotFoundException("Transaction not found");
            }

            transaction.BudgetID = dto.BudgetID;
            transaction.Type = dto.Type;
            transaction.Category = dto.Category;
            transaction.Description = dto.Description;
            transaction.Value = dto.Value; 
            transaction.TransactionDate = dto.TransactionDate;

            await _dbContext.SaveChangesAsync();

            return transaction.Id;
        }

        public async Task<Transaction> Delete(int id, int budgetId)
        {
            
            var budget = await GetBudgetById(budgetId);

            var transaction = await _dbContext.Transactions
                .FirstOrDefaultAsync(x => x.Id == id);

            if (transaction is null || transaction.BudgetID != budgetId)
            {
                _logger.LogError("Transaction not found");
                throw new NotFoundException("Transaction not found");
            }

            _dbContext.Transactions.Remove(transaction);
            await _dbContext.SaveChangesAsync();

            return transaction;
        }

        public async Task<Budget> GetBudgetById(int budgetId)
        {
            _logger.LogInformation($"GetBudgetById({budgetId}) budget method invoked");
            var userId =  (int)_userContextService.GetUserId;

            var budget =  await _dbContext.Budgets
                .Where(b => b.UserID == userId)
                .Include(t => t.Transactions)
                .FirstOrDefaultAsync(m => m.Id == budgetId);

            if (budget is null)
            {
                _logger.LogError("Budget not found");
                throw new NotFoundException("Budget not found");
            }

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User,
                budget,
                new ResourceOperationRequirement(ResourceOperation.Read)).Result;


            if (!authorizationResult.Succeeded)
            {
                _logger.LogError("Forbidden - Authorization error");
                throw new ForbidException("Forbidden - Authorization error");
            }

            var result =  _mapper.Map<Budget>(budget);
            return result;
        }
    }
}








        


       

            
                
            
        


        

        






