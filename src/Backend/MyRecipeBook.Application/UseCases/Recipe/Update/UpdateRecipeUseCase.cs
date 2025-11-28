using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.Recipe;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.Recipe.Update;

public class UpdateRecipeUseCase : IUpdateRecipeUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IRecipeUpdateOnlyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRecipeUseCase(
        ILoggedUser loggedUser,
        IUnitOfWork unitOfWork,
        IRecipeUpdateOnlyRepository repository)
    {
        _loggedUser = loggedUser;
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task Execute(long recipeId, RequestRecipeJson request)
    {
        Validate(request);

        var loggedUser = await _loggedUser.User();

        var recipe = await _repository.GetById(loggedUser, recipeId);

        if (recipe is null)
            throw new NotFoundException(ResourceMessagesException.RECIPE_NOT_FOUND);

        //
        // === CAMPOS SIMPLES ===
        //
        recipe.Title = request.Title;

        recipe.CookingTime = request.CookingTime.HasValue
            ? (Domain.Enums.CookingTime)request.CookingTime.Value
            : null;

        recipe.Difficulty = request.Difficulty.HasValue
            ? (Domain.Enums.Difficulty)request.Difficulty.Value
            : null;

        //
        // === INGREDIENTES ===
        //
        recipe.Ingredients.Clear();

        foreach (var item in request.Ingredients)
        {
            recipe.Ingredients.Add(new Domain.Entities.Ingredient
            {
                RecipeId = recipe.Id,
                Item = item,
                Active = true,
                CreatedOn = DateTime.UtcNow
            });
        }

        //
        // === INSTRUCTIONS ===
        //
        recipe.Instructions.Clear();

        var ordered = request.Instructions.OrderBy(i => i.Step).ToList();

        for (var i = 0; i < ordered.Count; i++)
        {
            recipe.Instructions.Add(new Domain.Entities.Instruction
            {
                RecipeId = recipe.Id,
                Step = i + 1,
                Text = ordered[i].Text,
                Active = true,
                CreatedOn = DateTime.UtcNow
            });
        }

        //
        // === DISH TYPES ===
        //
        recipe.DishTypes.Clear();

        foreach (var dish in request.DishTypes)
        {
            recipe.DishTypes.Add(new Domain.Entities.DishType
            {
                RecipeId = recipe.Id,
                Type = (Domain.Enums.DishType)dish,
                Active = true,
                CreatedOn = DateTime.UtcNow
            });
        }

        //
        // >>> RESTAURADO AQUI <<<
        //
        _repository.Update(recipe);

        await _unitOfWork.Commit();
    }



    private static void Validate(RequestRecipeJson request)
    {
        var result = new RecipeValidator().Validate(request);

        if (result.IsValid.IsFalse())
            throw new ErrorOnValidationException(
                result.Errors.Select(e => e.ErrorMessage).Distinct().ToList()
            );
    }
}
