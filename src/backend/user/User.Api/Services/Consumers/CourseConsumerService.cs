using MassTransit;
using SharedMessages.CourseMessages;
using Sqids;
using User.Api.Models.Repositories;
using User.Api.Services.Email;

namespace User.Api.Services.Consumers
{
    public class CourseConsumerService : IConsumer<CourseNoteMessage>, IConsumer<CourseCreatedMessage>
    {
        private readonly IUnitOfWork _uof;
        private readonly SqidsEncoder<long> _sqids;
        private readonly EmailService _emailService;

        public CourseConsumerService(IUnitOfWork uof, SqidsEncoder<long> sqids, 
            EmailService emailService)
        {
            _uof = uof;
            _emailService = emailService;
            _sqids = sqids;
        }

        public async Task Consume(ConsumeContext<CourseNoteMessage> context)
        {
            var userId = context.Message.UserId;
            var decodeUserId = _sqids.Decode(userId).SingleOrDefault();
            var profile = await _uof.profileReadOnly.ProfileByUserId(decodeUserId);
            var user = await _uof.userReadOnly.UserById(decodeUserId);

            var average = Math.Round((decimal)context.Message.Note! / context.Message.CourseNumber, 2);;

            profile.Note = average;

            _uof.profileWriteOnly.UpdateProfile(profile);
            await _uof.Commit();
        }

        public async Task Consume(ConsumeContext<CourseCreatedMessage> context)
        {
            var userId = _sqids.Decode(context.Message.UserId).Single();
            var profile = await _uof.profileReadOnly.ProfileByUserId(userId);

            profile.CourseNumber = profile.CourseNumber is null ? 1 : profile.CourseNumber + 1;

            _uof.profileWriteOnly.UpdateProfile(profile);
            await _uof.Commit();
        }
    }
}
