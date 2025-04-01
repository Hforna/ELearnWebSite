
using Azure.Messaging.ServiceBus;
using Course.Application.Services;
using Course.Domain.Repositories;
using Course.Domain.Services.Rest;
using Course.Infrastructure.Services.Azure;
using Sqids;

namespace Course.Api.BackgroundServices
{
    public class NotifyNewModuleCourseService : BackgroundService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceProvider _serviceProvider;
        private readonly SqidsEncoder<long> _sqids;

        public NotifyNewModuleCourseService(NewModuleProcessor processor, IServiceProvider serviceProvider, SqidsEncoder<long> sqids)
        {
            _serviceProvider = serviceProvider;
            _sqids = sqids;
            _processor = processor.GetProcessor();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += ProcessMessageAsync;
            _processor.ProcessErrorAsync += ProcessErrorAsync;

            await _processor.StartProcessingAsync(stoppingToken);
        }

        public async Task ProcessMessageAsync(ProcessMessageEventArgs eventArgs)
        {
            var body = long.Parse(eventArgs.Message.Body);

            var scope = _serviceProvider.CreateScope();

            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var module = await uow.moduleRead.ModuleById(body);
            
            if(module is not null)
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var usersId = await uow.enrollmentRead.GetCourseUsersId(module.CourseId);

                var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                foreach(long id in usersId)
                {
                    var userIdEncode = _sqids.Encode(id);
                    var userInfos = await userService.GetUserInfosById(userIdEncode);
                    var userEmail = userInfos.email;
                    var userName = userInfos.userName;

                    await emailService.SendEmail(userName, userEmail, "You got a new module on a course", 
                        $"the teacher added a new module on course {module.Course.Title}");
                }
            }
        }

        public async Task ProcessErrorAsync(ProcessErrorEventArgs args) => await Task.CompletedTask;

        ~NotifyNewModuleCourseService() => Dispose();

        public override void Dispose()
        {
            base.Dispose();

            GC.Collect();
        }
    }
}
