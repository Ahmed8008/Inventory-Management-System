using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventorySystem.Models
{
    public class Users
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string roles { get; set; }
        public int User_Id { get; set; }
        public string CompanyName { get; set; }
        public string ResetPasswordToken { get; set; }
        public DateTime ResetPasswordTokenExpiry { get; set; }
        public string VerificationCode { get; set; }
        public string UserStatus { get; set; }
    }
}