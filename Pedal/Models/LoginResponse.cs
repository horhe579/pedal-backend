namespace Pedal.Models
{
    public class LoginResponse
    {
        public required string Token { get; set; }
        public required string CarId { get; set; }
        public required string Email { get; set; }
    }
}
