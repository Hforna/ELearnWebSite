using User.Api.Models.Repositories;

namespace User.Api.BackgroundServices
{
    public class DeleteAccountService : BackgroundService
    {
        private readonly ILogger<DeleteAccountService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public DeleteAccountService(ILogger<DeleteAccountService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DeleteAccountService iniciado em {Time}.", DateTime.UtcNow);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var uof = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        var users = await uof.userReadOnly.GetUsersNotActive();
                        if (users == null || !users.Any())
                        {
                            _logger.LogInformation("None user inactivate was found");
                        }
                        else
                        {
                            foreach (var user in users)
                            {
                                if (user.TimeDisabled <= DateTime.UtcNow)
                                {
                                    uof.userWriteOnly.DeleteUser(user);
                                    await uof.Commit();
                                    _logger.LogInformation("User {UserId} deleted", user.Id);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while executing the DeleteAccountService service.");
                }
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
