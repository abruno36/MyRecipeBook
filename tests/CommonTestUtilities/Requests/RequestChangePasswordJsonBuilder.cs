using Bogus;
using MyRecipeBook.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestChangePasswordJsonBuilder
{
    public static RequestChangePasswordJson Build(int passwordLength = 10)
    {
        var validPassword = "Aa1!" + new string('x', Math.Max(4, passwordLength - 4));

        return new Faker<RequestChangePasswordJson>()
            .RuleFor(x => x.Password, f => validPassword)
            .RuleFor(x => x.NewPassword, f => validPassword);
    }
}
