using System.Text.Json.Serialization;

namespace Backend.Core.Entities;

public class Payment
{
    public int Id { get; set; }
    
    public int RentalId { get; set; }
    
    [JsonIgnore]
    public Rental Rental { get; set; } = null!;
    
    public decimal Amount { get; set; }
    
    public PaymentMethod Method { get; set; }
    
    public PaymentStatus Status { get; set; }
    
    public DateTime PaymentDate { get; set; }
    
    public string? TransactionId { get; set; }
}

public enum PaymentMethod
{
    Cash,
    CreditCard,
    DebitCard,
    BankTransfer
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Refunded
}
