using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyRecipeBook.Domain.Dtos;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories.Recipe;

namespace MyRecipeBook.Infrastructure.DataAccess.Repositories;

public sealed class RecipeRepository :
    IRecipeWriteOnlyRepository,
    IRecipeUpdateOnlyRepository,
    IRecipeReadOnlyRepository
{
    private readonly MyRecipeBookDbContext _dbContext;

    public RecipeRepository(MyRecipeBookDbContext dbContext)
        => _dbContext = dbContext;

    public async Task Add(Recipe recipe)
        => await _dbContext.Recipes.AddAsync(recipe);

    public void Update(Recipe recipe)
        => _dbContext.Recipes.Update(recipe);

    public async Task Delete(long recipeId)
    {
        var recipe = await _dbContext.Recipes.FindAsync(recipeId);
        _dbContext.Recipes.Remove(recipe!);
    }

    public async Task<IList<Recipe>> Filter(User user, FilterRecipesDto filters)
    {
        IQueryable<Recipe> query = _dbContext.Recipes
            .AsNoTracking()
            .Include(r => r.Ingredients)
            .Include(r => r.DishTypes)
            .Where(r => r.Active && r.UserId == user.Id);

        if (filters.Difficulties.Any())
        {
            query = query.Where(r =>
                r.Difficulty.HasValue &&
                filters.Difficulties.Contains(r.Difficulty.Value));
        }

        if (filters.CookingTimes.Any())
        {
            query = query.Where(r =>
                r.CookingTime.HasValue &&
                filters.CookingTimes.Contains(r.CookingTime.Value));
        }

        if (filters.DishTypes.Any())
        {
            query = query.Where(r =>
                r.DishTypes.Any(d =>
                    filters.DishTypes.Contains(
                        (MyRecipeBook.Domain.Enums.DishType)d.Type
                    )
                )
            );
        }

        if (filters.RecipeTitle_Ingredient.NotEmpty())
        {
            var text = filters.RecipeTitle_Ingredient;

            query = query.Where(r =>
                EF.Functions.Like(r.Title, $"%{text}%") ||
                r.Ingredients.Any(i =>
                    i.Item != null &&
                    EF.Functions.Like(i.Item, $"%{text}%")));
        }

        return await query.ToListAsync(); //neste momento vai no banco de dados executar a query definida 
    }

    async Task<Recipe?> IRecipeReadOnlyRepository.GetById(
        User user,
        long recipeId)
    {
        return await GetFullRecipe()
            .AsNoTracking()
            .FirstOrDefaultAsync(r =>
                r.Active &&
                r.Id == recipeId &&
                r.UserId == user.Id);
    }

    async Task<Recipe?> IRecipeUpdateOnlyRepository.GetById(
        User user,
        long recipeId)
    {
        return await GetFullRecipe()
            .FirstOrDefaultAsync(r =>
                r.Active &&
                r.Id == recipeId &&
                r.UserId == user.Id);
    }

    public async Task<IList<Recipe>> GetForDashboard(User user)
    {
        return await _dbContext
            .Recipes
            .AsNoTracking()
            .Include(recipe => recipe.Ingredients)
            .Where(recipe => recipe.Active && recipe.UserId == user.Id)
            .OrderByDescending(r => r.CreatedOn)
            .Take(5)
            .ToListAsync();
    }

    private IIncludableQueryable<Recipe, IList<DishType>> GetFullRecipe()
    {
        return _dbContext.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Instructions)
            .Include(r => r.DishTypes);
    }
}
