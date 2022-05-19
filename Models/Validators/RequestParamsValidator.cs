using FluentValidation;
using HomeBudget.Entities;

namespace HomeBudget.Models.Validators
{
    public class RequestParamsValidator : AbstractValidator<RequestParams>
    {
        private string[] allowedSortByColumnNames = { 
            nameof(Transaction.Value),
            nameof(Transaction.TransactionDate),
            nameof(Transaction.Type) };

        public RequestParamsValidator()
        {
            RuleFor(r => r.SortBy)
                .Must(value => string.IsNullOrEmpty(value) || allowedSortByColumnNames.Contains(value))
                .WithMessage("It's possible to sort only by: Value, Transaction Date and Type");
        }

    }
}
