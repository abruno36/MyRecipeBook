using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Services.Storage;

namespace MyRecipeBook.Infrastructure.Services.Storage;

public class FakeBlobStorageService : IBlobStorageService
{
    public Task Upload(User user, Stream stream, string fileName)
    {
        // Apenas ignora, simulando sucesso.
        Console.WriteLine($"[FAKE STORAGE] Upload ignorado → {fileName}");
        return Task.CompletedTask;
    }

    public Task<string> GetFileUrl(User user, string fileName)
    {
        // Retorna um link fictício só para a aplicação não quebrar.
        var url = $"https://fake-storage/{user.UserIdentifier}/{fileName}";
        return Task.FromResult(url);
    }

    public Task Delete(User user, string fileName)
    {
        // Simula exclusão
        Console.WriteLine($"[FAKE STORAGE] Delete ignorado → {fileName}");
        return Task.CompletedTask;
    }

    public Task DeleteContainer(Guid userIdentifier)
    {
        // Simula exclusão do container
        Console.WriteLine($"[FAKE STORAGE] Delete Container ignorado → {userIdentifier}");
        return Task.CompletedTask;
    }
}

