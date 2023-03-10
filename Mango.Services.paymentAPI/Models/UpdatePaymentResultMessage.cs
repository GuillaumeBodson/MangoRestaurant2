using AzureMessageBus;

namespace Mango.Services.paymentAPI.Models;

public class UpdatePaymentResultMessage : BaseMessage
{
    public int OrderId { get; set; }
    public bool Status { get; set; }
    public string Email { get; set; }
}
