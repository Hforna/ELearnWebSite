using Payment.Domain.Exceptions;
using Payment.Domain.Services.Session;
using System.Text;
using System.Text.Json;

namespace Payment.Api.Sessions
{
    public class OrderSessionService : IOrderSessionService
    {
        private readonly ISession _session;

        public OrderSessionService(ISession session)
        {
            _session = session;
        }

        public void AddOrderToSession(long courseId)
        {
            if(_session.TryGetValue("user_order", out var orderItems))
            {
                var toString = Encoding.UTF8.GetString(orderItems);
                var deserialize = JsonSerializer.Deserialize<Dictionary<Guid, long>>(toString);

                if (deserialize.ContainsValue(courseId))
                {
                    throw new OrderException(ResourceExceptMessages.ORDER_ITEM_EXISTS);
                }

                deserialize!.Add(Guid.NewGuid(), courseId);

                var serialize = JsonSerializer.Serialize(deserialize);
                _session.SetString("user_order", serialize);
            } else
            {
                var dict = new Dictionary<Guid, long>();
                dict.Add(Guid.NewGuid(), courseId);

                _session.SetString("user_order", JsonSerializer.Serialize(dict));
            }
        }

        public Dictionary<Guid, long>? GetSessionOrder()
        {
            if(_session.TryGetValue("user_order", out var value))
            {
                var deserializer = JsonSerializer.Deserialize<Dictionary<Guid, long>>(value);

                return deserializer;
            }
            return null;
        }
    }
}
