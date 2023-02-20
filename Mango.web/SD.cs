
namespace Mango.web
{
    public class SD
    {
        public static string ProductApiBase { get; set; }
        public static string ShoppingCartAPI { get; set; }
        public static string CouponAPIBase { get; set; }
        public enum ApiType
        {
            Get,
            Post,
            Put,
            Delete,
        }
    }
}
