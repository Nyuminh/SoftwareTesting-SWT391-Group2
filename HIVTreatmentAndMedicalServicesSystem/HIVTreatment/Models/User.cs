﻿namespace HIVTreatment.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string Fullname { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string? Image { get; set; }
    }
}
