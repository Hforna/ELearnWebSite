
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Infrastructure.Data;

namespace Course.Api.BackgroundServices
{
    public class DeleteModuleService : BackgroundService
    {
        private readonly IStorageService _storageService;
        private readonly IUnitOfWork _uof;

        public DeleteModuleService(IStorageService storageService, IUnitOfWork uof)
        {
            _storageService = storageService;
            _uof = uof;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                var modules = await _uof.moduleRead.GetNotActiveModules();

                if(modules.Count != 0 || modules is not null)
                {
                    foreach (var module in modules)
                    {
                        if (module.Lessons.Count != 0)
                        {
                            foreach (var lesson in module.Lessons)
                            {
                                var video = await _uof.videoRead.VideoById(lesson.VideoId);

                                await _storageService.DeleteVideo(module.Course.courseIdentifier, video.Id);
                                await _storageService.DeleteThumbnailVideo(video.Id);

                                await _uof.videoWrite.DeleteVideo(video.Id);
                            }
                            _uof.lessonWrite.DeleteLessonRange(module.Lessons);
                        }
                        _uof.moduleWrite.DeleteModule(module);
                    }
                    await _uof.Commit();
                }

                await Task.Delay(TimeSpan.FromDays(1));
            }
        }
    }
}
