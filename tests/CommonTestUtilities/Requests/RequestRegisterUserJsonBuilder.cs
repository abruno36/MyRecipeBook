using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestRegisterUserJsonBuilder
{
    public static RequestRegisterUserJson Build(int passwordLength = 10)
    {
        // Gera uma senha válida: maiúscula + minúscula + número + caractere especial
        var validPassword = "Aa1!" + new string('x', Math.Max(4, passwordLength - 4));

        return new Faker<RequestRegisterUserJson>()
            .RuleFor(u => u.Name, f => f.Person.FullName)
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Password, f => validPassword);
    }
}
