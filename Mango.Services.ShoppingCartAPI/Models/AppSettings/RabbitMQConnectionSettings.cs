namespace Mango.Services.ShoppingCartAPI.Models.AppSettings
{
    public class RabbitMQConnectionSettings
    {
        public string HostName { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string QueueCheckoutMessageName { get; set; }
    }
}
