using AutoMapper;
using HomeBudget.Authorization;
using HomeBudget.Entities;
using HomeBudget.Exceptions;
using HomeBudget.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HomeBudget.Services
{
    public interface IBudgetService
    {
        Task<int> Create(CreateBudgetModel dto);
        Task<Budget> Delete(int id);
        Task<IEnumerable<BudgetModel>> GetAll();
        Task<BudgetModel> GetById(int id);
        Task<int> Update(int id, UpdateBudget dto);
    }

    public class BudgetService : IBudgetService
    {
        private readonly BudgetDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;
        private readonly ILogger<BudgetService> _logger;

        public BudgetService(BudgetDbContext dbContext,
            IMapper mapper,
            IAuthorizationService authorizationService, 
            IUserContextService userContextService, 
            ILogger<BudgetService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
            _logger = logger;
        }

        public async Task<BudgetModel> GetById(int id)
        {
            
            var userId = (int)_userContextService.GetUserId;

            var budget = await _dbContext.Budgets
                .Where(b => b.UserID == userId)
                .Include(t => t.Transactions)
                .FirstOrDefaultAsync(m => m.Id == id);

            if(budget is null)
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


            var result = _mapper.Map<BudgetModel>(budget);
            return result;
        }




        public async Task<IEnumerable<BudgetModel>> GetAll()
        {
            
            var userId = (int)_userContextService.GetUserId;

            var budget =  await _dbContext.Budgets
                .Where(b => b.UserID == userId)
                .Include(t => t.Transactions)
                .OrderBy(n => n.Name)
                .ToListAsync();


            if (budget is null)
            {
                _logger.LogError("Budget not found");
                throw new NotFoundException("Budget not found");
            }


            var budgetDtos = _mapper.Map<List<BudgetModel>>(budget);

            return budgetDtos;
        }



        public async Task<int> Create(CreateBudgetModel dto)
        {
            
            var budget = _mapper.Map<Budget>(dto);
            budget.UserID = (int)_userContextService.GetUserId;

            
            await _dbContext.Budgets.AddAsync(budget);
            await _dbContext.SaveChangesAsync();

            return budget.Id;
        }


        public async Task<Budget> Delete(int id)
        {
            
            var budget = await _dbContext.Budgets
                .FirstOrDefaultAsync(x => x.Id == id);

            if(budget is null)
            {
                _logger.LogError("Budget not found");
                throw new NotFoundException("Budget not found");
            }

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User,
                budget,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;
            

            if(!authorizationResult.Succeeded)
            {
                _logger.LogError("Forbidden - Authorization error");
                throw new ForbidException("Forbidden - Authorization error");
            }

             _dbContext.Budgets.Remove(budget);
            await _dbContext.SaveChangesAsync();

            return budget;

        }

        public async Task<int> Update(int id, UpdateBudget dto)
        {
            
            var budget = await _dbContext.Budgets
                .FirstOrDefaultAsync(x => x.Id == id);


            if (budget is null)
            {
                _logger.LogError("Budget not found");
                throw new NotFoundException("Budget not found");
            }

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User,
                budget,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;


            if (!authorizationResult.Succeeded)
            {
                _logger.LogError("Forbidden - Authorization error");
                throw new ForbidException("Forbidden - Authorization error");
            }


            budget.Name = dto.Name;
            budget.Description = dto.Description;

            await _dbContext.SaveChangesAsync();

            return budget.Id;

        }


    }
}

            
