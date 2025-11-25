using FluentValidation;
using FluentValidation.Validators;
using MyRecipeBook.Exceptions;

namespace MyRecipeBook.Application.SharedValidators;

public class PasswordValidator<T> : PropertyValidator<T, string>
{
    private const string ErrorMessageKey = "ErrorMessage";
    public override bool IsValid(ValidationContext<T> context, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            context.MessageFormatter.AppendArgument(ErrorMessageKey, ResourceMessagesException.PASSWORD_EMPTY);

            return false;
        }

        if (password.Length < 8)
        {
            context.MessageFormatter.AppendArgument(ErrorMessageKey, ResourceMessagesException.INVALID_PASSWORD);

            return false;
        }

        // Pelo menos 1 letra maiúscula
        if (!password.Any(char.IsUpper))
        {
            context.MessageFormatter.AppendArgument(ErrorMessageKey, ResourceMessagesException.PASSWORD_NEEDS_UPPERCASE);
            return false;
        }

        // Pelo menos 1 caractere especial
        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
        {
            context.MessageFormatter.AppendArgument(ErrorMessageKey, ResourceMessagesException.PASSWORD_NEEDS_SPECIAL_CHAR);
            return false;
        }

        return true;
    }

    public override string Name => "PasswordValidator";

    protected override string GetDefaultMessageTemplate(string errorCode) => "{ErrorMessage}";
}
