using CommonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Exceptions;
using Xunit;

namespace Validators.Test.User.ChangePassword;

public class ChangePasswordValidatorTest
{
    [Fact]
    public void Success()
    {
        var validator = new ChangePasswordValidator();

        // Criamos uma senha válida: >=8 chars, 1 maiúscula, 1 especial
        var request = RequestChangePasswordJsonBuilder.Build(passwordLength: 10);
        request.NewPassword = "Abcdef!1";

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Error_Password_Empty()
    {
        var validator = new ChangePasswordValidator();

        var request = RequestChangePasswordJsonBuilder.Build();
        request.NewPassword = string.Empty;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();

        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.PASSWORD_EMPTY));
    }

    [Theory]
    [InlineData("aA!")]         // muito curta (<8)
    [InlineData("abcdEF!")]     // 7 caracteres
    public void Error_Password_Invalid_Length(string password)
    {
        var validator = new ChangePasswordValidator();

        var request = RequestChangePasswordJsonBuilder.Build();
        request.NewPassword = password;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();

        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.INVALID_PASSWORD));
    }

    [Theory]
    [InlineData("abcdef!1")]  // sem maiúscula
    [InlineData("senha@123")] // sem maiúscula
    public void Error_Password_Needs_Uppercase(string password)
    {
        var validator = new ChangePasswordValidator();

        var request = RequestChangePasswordJsonBuilder.Build();
        request.NewPassword = password;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();

        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.PASSWORD_NEEDS_UPPERCASE));
    }

    [Theory]
    [InlineData("Abcdefg1")]  // sem caractere especial
    [InlineData("Senha123")]  // sem caractere especial
    public void Error_Password_Needs_Special_Char(string password)
    {
        var validator = new ChangePasswordValidator();

        var request = RequestChangePasswordJsonBuilder.Build();
        request.NewPassword = password;

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();

        result.Errors.Should().ContainSingle()
            .And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesException.PASSWORD_NEEDS_SPECIAL_CHAR));
    }
}
