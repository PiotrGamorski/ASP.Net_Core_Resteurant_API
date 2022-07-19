using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Resteurant_API.DataContext;
using Resteurant_API.Dtos;

namespace Resteurant_API.Validators
{
    public class CreateDishDtoValidator : AbstractValidator<CreateDishDto>
    {
        private readonly ResteurantDbContext _dbContext;

        public CreateDishDtoValidator(ResteurantDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Name)
                .Custom((value, context) =>
                {
                    //var resteurants = _dbContext.Resteurants.Include(r => r.Dishes).ToArrayAsync();
                });
        }
    }
}
