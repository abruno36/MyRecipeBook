namespace MyRecipeBook.Domain.Security.Tokens
{
    public interface ITokenService
    {
        Task<(string accessToken, string refreshToken)> GenerateTokensAsync(Domain.Entities.User user);
    }
}
