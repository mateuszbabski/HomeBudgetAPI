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
        PagedList<TransactionModel> GetAll(int budgetId, RequestParams request);
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
        

        public PagedList<TransactionModel> GetAll(int budgetId, RequestParams request)
        {
            var budget = GetBudgetById(budgetId);

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

            var transactions = baseQuery
                //.OrderByDescending(x => x.TransactionDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var totalCount = baseQuery.Count();

            var transactionsDto = _mapper.Map<List<TransactionModel>>(transactions);

            var result = new PagedList<TransactionModel>(transactionsDto, totalCount, request.PageNumber, request.PageSize);
            

            return result;
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
            var userId =  (int)_userContextService.GetUserId;

            var budget =  _dbContext.Budgets
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

            var result =  _mapper.Map<Budget>(budget);
            return result;
        }
    }
}







        


       

            
                
            
        


        

        






