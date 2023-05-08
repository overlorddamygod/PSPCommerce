namespace PSPCommerce.DTO
{
    public class CreateIntentReq {
        public int amount { get; set; }
    }

    public class VerifyPayReq {
        public string paymentId { get; set; }
    }
}