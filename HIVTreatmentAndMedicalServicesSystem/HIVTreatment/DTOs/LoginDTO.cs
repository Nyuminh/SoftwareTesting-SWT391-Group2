namespace HIVTreatment.DTOs
{
    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginResponse
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
