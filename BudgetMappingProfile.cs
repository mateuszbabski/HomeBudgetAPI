using AutoMapper;
using HomeBudget.Entities;
using HomeBudget.Models;

namespace HomeBudget
{
    public class BudgetMappingProfile : Profile
    {
        public BudgetMappingProfile()
        {
            //CreateMap<A, B> - create map from A to B 
            CreateMap<Transaction, TransactionModel>();
                

            CreateMap<CreateTransactionModel, Transaction>();
                

            CreateMap<Budget, BudgetModel>();

            CreateMap<CreateBudgetModel, Budget>();

            
        }
    }
}

