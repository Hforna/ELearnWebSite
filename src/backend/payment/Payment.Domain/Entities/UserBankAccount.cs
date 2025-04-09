using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Entities
{
    [Table("user-bank-accounts")]
    public class UserBankAccount
    {
        [Key]
        public Guid Id { get; set; }
        public long TeacherId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string AgencyNumber { get; set; }
        public string AccountNumber { get; set; }
        public string BankCode { get; set; }
        public string TaxId { get; set; }
        public string TypeAccount { get; set; }
        public string? PixKey { get; set; }
    }
}
