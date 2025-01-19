using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Infrastructure.Data;
using Course.Infrastructure.Services.Azure;

namespace Course.Api.BackgroundServices
{
    public class DeleteModuleService : BackgroundService
    {
        private readonly IServiceProvider _provider;

        public DeleteModuleService(IServiceProvider provider)
        {
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _provider.CreateScope();
                    var storageService = scope.ServiceProvider.GetRequiredService<IStorageService>();
                    var uof = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var modules = await uof.moduleRead.GetNotActiveModules();

                    if (modules.Count != 0 && modules is not null)
                    {
                        foreach (var module in modules)
                        {
                            if (module.Lessons.Count != 0)
                            {
                                foreach (var lesson in module.Lessons)
                                {
                                    var video = await uof.videoRead.VideoById(lesson.VideoId);

                                    await storageService.DeleteVideo(module.Course.courseIdentifier, video.Id);
                                    await storageService.DeleteThumbnailVideo(video.Id);

                                    await uof.videoWrite.DeleteVideo(video.Id);
                                }
                                uof.lessonWrite.DeleteLessonRange(module.Lessons);
                            }
                            uof.moduleWrite.DeleteModule(module);
                        }
                        await uof.Commit();
                    }
                }
                catch(System.Exception ex)
                {
                    Console.WriteLine($"Error on delete module process: {ex}");
                }
                await Task.Delay(TimeSpan.FromDays(1));
            }
        }
    }
}
