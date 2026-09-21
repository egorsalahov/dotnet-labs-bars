using FluentValidation;
using RestApiHomework.DTO_s;

namespace RestApiHomework.Validators
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Нужно имя пользователя")
                .MinimumLength(3).WithMessage("В имени должно быть хотя-бы 3 знака");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Нужна почта")
                .EmailAddress().WithMessage("Невалидная почта");
        }
    }
}
