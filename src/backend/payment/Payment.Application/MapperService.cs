using AutoMapper;
using Payment.Application.Responses.Order;
using Payment.Application.Responses.Payment;
using Payment.Domain.DTOs;
using Payment.Domain.Entities;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application
{
    public class MapperService : Profile
    {
        private readonly SqidsEncoder<long> _sqids;
        public MapperService(SqidsEncoder<long> sqids)
        {
            _sqids = sqids;

            RequestToEntity();
            EntityToResponse();
            DtoToResponse();
        }

        void RequestToEntity()
        {
            
        }

        void EntityToResponse()
        {
            CreateMap<OrderItem, OrderItemResponse>()
                .ForMember(d => d.CourseId, f => f.MapFrom(d => _sqids.Encode(d.CourseId)));

            CreateMap<Order, OrderResponse>()
                .ForMember(d => d.UserId, f => f.MapFrom(d => _sqids.Encode(d.UserId)));
        }

        void DtoToResponse()
        {
            CreateMap<PixPaymentResponseDto, PaymentPixResponse>();
        }
    }
}
