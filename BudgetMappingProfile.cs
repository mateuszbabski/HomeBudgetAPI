using AutoMapper;
using HomeBudget.Entities;
using HomeBudget.Models;

namespace HomeBudget
{
    public class BudgetMappingProfile : Profile
    {
        public BudgetMappingProfile()
        {
            CreateMap<Transaction, TransactionModel>();

            CreateMap<CreateTransactionModel, Transaction>();

            CreateMap<Budget, BudgetModel>();

            CreateMap<CreateBudgetModel, Budget>();

        }
    }
}
