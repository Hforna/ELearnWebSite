using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Requests
{
    public class TrioWebhookPayload
    {
        public string Category { get; set; } = null!;
        public string CompanyId { get; set; } = null!;
        public PaymentData Data { get; set; } = null!;
        public string? RefId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type { get; set; } = null!;
    }

    public class PaymentData
    {
        public Counterparty? Counterparty { get; set; }
        public BankAccount? CounterpartyBankAccount { get; set; }
        public string? CounterpartyBankAccountId { get; set; }
        public string? CounterpartyId { get; set; }
        public string EndToEndId { get; set; } = null!;
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string ExternalId { get; set; } = null!;
        public string Id { get; set; } = null!;
        public string IntegrationStatus { get; set; } = null!;
        public string PaymentDocumentId { get; set; } = null!;
        public string? ReceiptUrl { get; set; }
        public DateTime Timestamp { get; set; }
        public string Type { get; set; } = null!;
    }

    public class Counterparty
    {
        public string CompanyId { get; set; } = null!;
        public string? ExternalId { get; set; }
        public string Id { get; set; } = null!;
        public DateTime InsertedAt { get; set; }
        public string LedgerType { get; set; } = null!;
        public string? LegalName { get; set; }
        public decimal? MaximumAmount { get; set; }
        public int? MaximumTransactions { get; set; }
        public string Name { get; set; } = null!;
        public string TaxNumber { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
    }

    public class BankAccount
    {
        public string AccountDigit { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public string AccountType { get; set; } = null!;
        public string BankIspb { get; set; } = null!;
        public string BankNumber { get; set; } = null!;
        public string Branch { get; set; } = null!;
        public string Id { get; set; } = null!;
        public DateTime InsertedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
