using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Services.ServiceBus;

namespace MyRecipeBook.Infrastructure.Services.ServiceBus
{
    public class FakeDeleteUserQueue : IDeleteUserQueue
    {
        public Task SendMessage(User user)
        {
            Console.WriteLine($"[FakeQueue] (fake) solicitando deleção do usuário: {user?.Id}");
            return Task.CompletedTask;
        }
    }
}
