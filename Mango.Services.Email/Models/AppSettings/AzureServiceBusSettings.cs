namespace Mango.Services.Email.Models.AppSettings
{
    public class AzureServiceBusSettings
    {
        public string ConnectionString { get; set; }
        public string SubscriptionName { get; set; }
        public string OrderUpdatePaymentResultTopic { get; set; }
    }
}
