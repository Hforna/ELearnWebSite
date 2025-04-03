using MassTransit;
using Microsoft.AspNetCore.Identity;
using SharedMessages.UserMessages;
using User.Api.Models;
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
            _logger.LogInformation("DeleteAccountService started on {Time}.", DateTime.UtcNow);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var uof = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                        var bus = scope.ServiceProvider.GetRequiredService<IBus>();
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserModel>>();

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
                                    var roles = await userManager.GetRolesAsync(user);
                                    await bus.Publish(new UserDeletedMessage() { UserId = user.Id, 
                                        Teacher = roles.Contains("teacher") }, 
                                        d => d.SetRoutingKey("user.deleted"));

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
