using FluentValidation;
using Resteurant_API.DataContext;
using Resteurant_API.Dtos;
using System.Linq;

namespace Resteurant_API.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        private readonly ResteurantDbContext _dbContext;

        public RegisterUserDtoValidator(ResteurantDbContext context)
        {
            _dbContext = context;

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password);

            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    var emailInUse = _dbContext.Users.Any(u => u.Email == value);
                    if (emailInUse)
                        context.AddFailure("Email", "That email is already taken");
                });
        }
    }
}
