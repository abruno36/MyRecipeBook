namespace MyRecipeBook.Domain.Dtos;

public sealed class RecipeDashboardDto
{
    public long Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int AmountIngredients { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
}
