namespace Mango.Services.paymentAPI.Models.AppSettings
{
    public class AzureServiceBusSettings
    {
        public string ConnectionString { get; set; }
        public string OrderUpdatePaymentResultTopic { get; set; }
        public string OrderPaymentProcessTopic { get; set; }
        public string OrderPaymentProcessSubscription { get; set; }
    }
}
