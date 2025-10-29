namespace MyRecipeBook.Communication.Responses;
public class ResponseRegisteredUserJson
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ResponseTokensJson Tokens { get; set; } = default!;
}
