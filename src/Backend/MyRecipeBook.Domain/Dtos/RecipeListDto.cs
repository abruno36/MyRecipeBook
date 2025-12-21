using MyRecipeBook.Domain.Enums;

namespace MyRecipeBook.Domain.Dtos;

public class RecipeListDto
{
    public long Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int? Difficulty { get; init; }
    public int? CookingTime { get; init; }
}
