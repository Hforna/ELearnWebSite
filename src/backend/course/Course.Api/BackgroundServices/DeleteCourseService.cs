using Azure.Messaging.ServiceBus;
using Course.Application.UseCases.Repositories.Course;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Infrastructure.Services.Azure;
using Microsoft.AspNetCore.Http.Extensions;

namespace Course.Api.BackgroundServices
{
    public class DeleteCourseService : BackgroundService
    {
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceProvider _provider;

        public DeleteCourseService(IServiceProvider provider, DeleteCourseProcessor processor)
        {
            _processor = processor.GetProcessor();
            _provider = provider;
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
            var uof = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var storage = scope.ServiceProvider.GetRequiredService<IStorageService>();

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
            uof.reviewWrite.DeleteCourseReviews(course.Id);
            await uof.Commit();
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
