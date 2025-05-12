using SharedMessages.CourseMessages;

namespace Course.Domain.Services.RabbitMq
{
    public interface IProducerService
    {
        public Task SendCourseNote(CourseNoteMessage message);
        public Task SendCourseCreated(CourseCreatedMessage message);
    }
}