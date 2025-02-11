using Azure.Messaging.ServiceBus;
using Course.Application.UseCases.Repositories.Course;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Infrastructure.Services.Azure;

namespace Course.Api.BackgroundServices
{
    public class DeleteCourseService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly ServiceBusProcessor _processor;

        public DeleteCourseService(IServiceProvider provider, DeleteCourseProcessor processor)
        {
            _provider = provider;
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

            var scope = _provider.CreateScope();
            var uof = _provider.GetRequiredService<IUnitOfWork>();
            var storage = _provider.GetRequiredService<IStorageService>();

            var course = await uof.courseRead.CourseById(body);

            await storage.DeleteCourseImage(course.courseIdentifier, course.Thumbnail);
            uof.courseWrite.DeleteCourse(course);
            foreach(var module in course.Modules)
            {
                foreach(var lesson in module.Lessons)
                {
                    await storage.DeleteVideo(course.courseIdentifier, lesson.VideoId);
                    await uof.videoWrite.DeleteVideo(lesson.VideoId);
                }
                uof.lessonWrite.DeleteLessonRange(module.Lessons);
                uof.moduleWrite.DeleteModule(module);
            }
        }

        public async Task ProcessErrorAsync(ProcessErrorEventArgs args) => await Task.CompletedTask;

        ~DeleteCourseService() => Dispose();

        public override void Dispose()
        {
            base.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
