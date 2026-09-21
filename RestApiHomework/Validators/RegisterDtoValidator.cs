using FluentValidation;
using RestApiHomework.DTO_s;

namespace RestApiHomework.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Имя пользователя обязательно")
                .MinimumLength(3).WithMessage("В имени нужно минимально 3 знака")
                .MaximumLength(50).WithMessage("В имени нужно меньше 50 знаков");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Почта обязателена")
                .EmailAddress().WithMessage("Невалидная почта");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен")
                .MinimumLength(6).WithMessage("В пароле должно быть не меньше 6 знаков");
        }
    }
}
