using CommonTestUtilities.BlobStorage;
using CommonTestUtilities.Entities;
using CommonTestUtilities.LoggedUser;
using CommonTestUtilities.Mapper;
using CommonTestUtilities.Repositories;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.Dashboard;
using Xunit;

namespace UseCases.Test.Dashboard;

public class GetDashboardUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        // Arrange
        (var user, _) = UserBuilder.Build();

        var entityRecipes = RecipeBuilder.Collection(user);

        var useCase = CreateUseCase(user, entityRecipes);

        // Act
        var result = await useCase.Execute();

        // Assert
        result.Should().NotBeNull();
        result.Recipes.Should()
            .NotBeNullOrEmpty()
            .And.AllSatisfy(recipe =>
            {
                recipe.Id.Should().NotBeNullOrWhiteSpace();
                recipe.Title.Should().NotBeNullOrWhiteSpace();
                recipe.AmountIngredients.Should().BeGreaterThan(0);
                recipe.ImageUrl.Should().NotBeNullOrWhiteSpace();
            });
    }

    // =========================
    // Factory
    // =========================

    private static GetDashboardUseCase CreateUseCase(
        MyRecipeBook.Domain.Entities.User user,
        IList<MyRecipeBook.Domain.Entities.Recipe> entityRecipes)
    {
        var mapper = MapperBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);

        var repository = new RecipeReadOnlyRepositoryBuilder()
            .GetForDashboard(user, entityRecipes)
            .Build();

        var blobStorage = new BlobStorageServiceBuilder()
            .GetFileUrl(user, entityRecipes)
            .Build();

        return new GetDashboardUseCase(
            repository,
            mapper,
            loggedUser,
            blobStorage);
    }
}
