using Course.Domain.Repositories;
using Course.Domain.Services.Azure;

namespace Course.Api.BackgroundServices
{
    public class DeleteCourseService : BackgroundService
    {
        private readonly IServiceProvider _provider;

        public DeleteCourseService(IServiceProvider provider) => _provider = provider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {


                    var scope = _provider.CreateScope();

                    var uof = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var storage = scope.ServiceProvider.GetRequiredService<IStorageService>();

                    var courses = await uof.courseRead.GetNotActiveCourses();

                    if (courses.Count != 0)
                    {
                        foreach (var course in courses)
                        {
                            await storage.DeleteCourseImage(course.courseIdentifier, course.Thumbnail);    
                        }
                        uof.courseWrite.DeleteCourseRange(courses);
                        await uof.Commit();
                    }
                } catch(System.Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                await Task.Delay(TimeSpan.FromDays(1));
            }
        }
    }
}
