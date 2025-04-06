using Microsoft.Extensions.Hosting;
using Payment.Domain.Repositories;

namespace Payment.Api.BackgroundServices
{
    public class AllowBalanceService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AllowBalanceService> _logger;

        public AllowBalanceService(IServiceProvider serviceProvider, ILogger<AllowBalanceService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var uof = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var blockedBalances = await uof.balanceRead.GetAllBlockedBalancesGroupedByBalance();

                    if (blockedBalances is not null)
                    {
                        foreach (var blockeds in blockedBalances)
                        {
                            var balance = await uof.balanceRead.BalanceById(blockeds.Key);
                            var balanceAmount = balance.AvaliableBalance;
                            _logger.LogInformation($"Current user balance: {balanceAmount}");

                            foreach (var blocked in blockeds.Value)
                            {
                                if (blocked.CreatedAt.AddDays(7) <= DateTime.UtcNow)
                                {
                                    balanceAmount += blocked.Amount;
                                    _logger.LogInformation($"Amount added to user balance: {blocked.Amount}");
                                    uof.balanceWrite.DeleteBlockedBalance(blocked);
                                }
                            }
                            uof.balanceWrite.Update(balance);
                            await uof.Commit();
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
