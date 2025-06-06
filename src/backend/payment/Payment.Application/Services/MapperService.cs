﻿using AutoMapper;
using Payment.Application.Requests;
using Payment.Application.Responses.Balance;
using Payment.Application.Responses.Order;
using Payment.Application.Responses.Payment;
using Payment.Domain.DTOs;
using Payment.Domain.Entities;
using Sqids;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Services
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
            CreateMap<CreateBankAccountRequest, UserBankAccount>()
                .ForMember(d => d.TeacherId, f => f.Ignore());
        }

        void EntityToResponse()
        {
            CreateMap<OrderItem, OrderItemResponse>()
                .ForMember(d => d.CourseId, f => f.MapFrom(d => _sqids.Encode(d.CourseId)));

            CreateMap<Order, OrderResponse>()
                .ForMember(d => d.UserId, f => f.MapFrom(d => _sqids.Encode(d.UserId)));

            CreateMap<Domain.Entities.Balance, BalanceResponse>()
                .ForMember(d => d.UserId, f => f.MapFrom(d => _sqids.Encode(d.TeacherId)));
        }

        void DtoToResponse()
        {
            CreateMap<PixPaymentResponseDto, PaymentPixResponse>();

            CreateMap<StripeDebitDto, PaymentCardResponse>();

            CreateMap<RefundService, RefundDto>();

            CreateMap<PaymentIntent, StripeDebitDto>()
                .ForMember(d => d.RequiresAction, f => f.Condition(d => d.Status == "requires_action"))
                .ForMember(d => d.Success, f => f.Condition(d => d.Status == "succeeded"));

            CreateMap<PaymentIntent, StripeCreditDto>()
                .ForMember(d => d.RequiresAction, f => f.Condition(d => d.Status == "requires_action"))
                .ForMember(d => d.Success, f => f.Condition(d => d.Status == "succeeded"))
                .ForMember(d => d.Installments, f => f.MapFrom(d => d.PaymentMethodOptions.Card.Installments.Plan.Count));
        }
    }
}
