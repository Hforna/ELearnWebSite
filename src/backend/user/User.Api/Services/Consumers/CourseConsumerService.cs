using AutoMapper;
using MassTransit;
using Sqids;
using User.Api.Models.Repositories;
using User.Api.Services.Consumers.ResponseClass;
using User.Api.Services.Email;

namespace User.Api.Services.Consumers
{
    public class CourseConsumerService : IConsumer<CourseNoteMessage>
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

            var average = Math.Round((decimal)context.Message.Note! / context.Message.CourseNumber, 2);

            await _emailService.SendEmail(user.Email, user.UserName, "new notify", $"you profile note now is: {average}");

            profile.Note = average;

            _uof.profileWriteOnly.UpdateProfile(profile);
            await _uof.Commit();
        }
    }
}
