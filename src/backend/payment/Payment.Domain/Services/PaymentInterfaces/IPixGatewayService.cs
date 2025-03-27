using Payment.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Payment.Domain.Services.PaymentInterfaces
{
    public interface IPixGatewayService
    {
        public Task<PixPaymentResponseDto> ProcessPixTransaction(string cpf, string email, string firstName, string lastName, decimal amount); 
    }
}
