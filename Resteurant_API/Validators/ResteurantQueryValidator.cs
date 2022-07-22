using FluentValidation;
using Resteurant_API.Entities;
using Resteurant_API.Models;
using System.Linq;

namespace Resteurant_API.Validators
{
    public class ResteurantQueryValidator : AbstractValidator<ResteurantQuery>
    {
        private string[] allowedSortByColumnNames =
            { nameof(Resteurant.Name), nameof(Resteurant.Description), nameof(Resteurant.Category)};

        public ResteurantQueryValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).Custom((value, context) =>
            {
                if (value % 5 != 0)
                {
                    int[] possiblePageSizes = { 5, 10, 15 };
                    context.AddFailure("PageSize", $"PageSize must be {string.Join(", ", possiblePageSizes)}");
                }
            });
            RuleFor(x => x.SortBy)
                .Must(value => string.IsNullOrEmpty(value) || allowedSortByColumnNames.Contains(value))
                .WithMessage($"SortBy is optional, or must be {string.Join(", ", allowedSortByColumnNames)}");
            RuleFor(x => x.SortDirection).Custom((value, context) =>
            {
                if (value.ToString() != null && value != SortDirection.ASC && value != SortDirection.DESC)
                { 
                    context.AddFailure($"SortDirection is optional, or must be {SortDirection.ASC} or {SortDirection.DESC}");
                }
            });
        }
    }
}
