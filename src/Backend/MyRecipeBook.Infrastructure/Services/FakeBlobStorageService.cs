using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Services.Storage;

namespace MyRecipeBook.Infrastructure.Services
{
    public class FakeBlobStorageService : IBlobStorageService
    {
        public Task Upload(User user, Stream stream, string fileName)
        {
            return Task.CompletedTask;
        }

        public Task<string> GetFileUrl(User user, string fileName)
        {
            var fakeUrl = $"https://fake-storage.local/{user?.Id}/{fileName}";
            return Task.FromResult(fakeUrl);
        }

        public Task Delete(User user, string fileName)
        {
            return Task.CompletedTask;
        }

        public Task DeleteContainer(Guid containerId)
        {
            return Task.CompletedTask;
        }
    }
}
