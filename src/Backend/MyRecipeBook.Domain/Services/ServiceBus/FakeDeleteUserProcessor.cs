namespace MyRecipeBook.Infrastructure.Services.ServiceBus
{
    public class FakeDeleteUserProcessor
    {
        public Task StartAsync()
        {
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            return Task.CompletedTask;
        }
    }
}
