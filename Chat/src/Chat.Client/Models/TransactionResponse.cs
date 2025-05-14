using System.Text.Json.Serialization;

namespace Chat.Client.Models;

public class TransactionsResponse
{
    public AccountInfo Account { get; set; } = new();
    public TransactionsContainer Transactions { get; set; } = new();

    public class AccountInfo
    {
        public string Iban { get; set; } = string.Empty;
        public string Bban { get; set; } = string.Empty;
        public string Pan { get; set; } = string.Empty;
        public string MaskedPan { get; set; } = string.Empty;
        public string Msisdn { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
    }

    public class TransactionsContainer
    {
        public List<Transaction> Booked { get; set; } = new();
    }

    public class Transaction
    {
        public string TransactionId { get; set; } = string.Empty;
        public string EntryReference { get; set; } = string.Empty;
        public string EndToEndId { get; set; } = string.Empty;
        public string BookingDate { get; set; } = string.Empty;
        public string ValueDate { get; set; } = string.Empty;
        public Amount TransactionAmount { get; set; } = new();
        public string CreditorName { get; set; } = string.Empty;
        public AccountRef CreditorAccount { get; set; } = new();
        public string DebtorName { get; set; } = string.Empty;
        public AccountRef DebtorAccount { get; set; } = new();
        public string RemittanceInformationStructured { get; set; } = string.Empty;
    }

    public class Amount
    {
        [JsonPropertyName("amount")]
        public string AmountValue { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
    }


    public class AccountRef
    {
        public string Iban { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
    }
}