using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using BCrypt.Net;
using InventorySystem.Models;

namespace InventorySystem.Controllers
{
    public class AccountsController : Controller
    {

        DataBase db = new DataBase();

        //
        // GET: /Accounts/
        [HttpGet]
        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]

        public ActionResult SignIn(Users u)
        {
            db.con.Open();
            string q = "SELECT * FROM [Users] WHERE Email=@Email";
            SqlCommand cmd = new SqlCommand(q, db.con);
            cmd.Parameters.AddWithValue("@Email", u.Email);

            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.Read())
            {
                string storedHashedPassword = sdr["Password"].ToString();

                if (BCrypt.Net.BCrypt.Verify(u.Password, storedHashedPassword))
                {
                    // Passwords match, sign in the user
                    string role = sdr["roles"].ToString();
                    Session["roles"] = role;
                    Session["Name"] = sdr["Name"].ToString();
                    Session["Email"] = u.Email;
                    Session["User_Id"] = sdr["User_Id"].ToString();
                    sdr.Close();
                    db.con.Close();

                    // Determine the role and redirect accordingly
                    if (role == "SuperAdmin")
                    {
                        return RedirectToAction("UserCreation", "UserManagement");
                    }
                    else if (role == "Admin")
                    {
                        return RedirectToAction("StockList", "Stock");
                    }
                    else if (role == "Worker")
                    {
                        return RedirectToAction("StockList", "Stock");
                    }
                    // Add more roles as needed
                }
            }

            // Incorrect email or password
            ViewBag.Result = "Email or Password is incorrect";
            sdr.Close();
            db.con.Close();
            return View();
        }


        [HttpGet]
        public ActionResult ForgetPassword()
        {
            return View();
        }
            [HttpPost]
            public ActionResult ForgetPassword(Users u)
{
    if (ModelState.IsValid)
    {
        // Check if the user exists with the provided email
        db.con.Open();
        string query = "SELECT * FROM Users WHERE Email = @Email";
        SqlCommand cmd = new SqlCommand(query, db.con);
        cmd.Parameters.AddWithValue("@Email", u.Email);

        SqlDataReader reader = cmd.ExecuteReader();

        Users user = null;
        if (reader.Read())
        {
            user = new Users
            {
                User_Id = Convert.ToInt32(reader["User_Id"])
                // Assuming other properties are not needed here for generating the link
            };
        }

        reader.Close();

        if (user != null)
        {
            // Generate a unique token for password reset
            string token = Guid.NewGuid().ToString();

            // Update the user's reset token and its expiry time in the database
            string updateQuery = "UPDATE Users SET ResetPasswordToken = @Token, ResetPasswordTokenExpiry = @Expiry WHERE Email = @Email";
            SqlCommand updateCmd = new SqlCommand(updateQuery, db.con);
            updateCmd.Parameters.AddWithValue("@Token", token);
            updateCmd.Parameters.AddWithValue("@Expiry", DateTime.Now.AddHours(1));
            updateCmd.Parameters.AddWithValue("@Email", u.Email);

            updateCmd.ExecuteNonQuery();

            // Send email with the password reset link
            var senderemail = new MailAddress("demo7899999@gmail.com", "Password Reset");
            var password = "bfapmnypsntylcde";

            var receiveremail = new MailAddress(u.Email, "Receiver");

            // Build the email body with the password reset link
            var callbackUrl = Url.Action("ResetPassword", "Accounts", new { userId = user.User_Id, token = token }, Request.Url.Scheme);
            string body = "Please reset your password by clicking <a href='" + callbackUrl + "'>here</a>";

            // Send the email
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderemail.Address, password)
            };

            using (var mess = new MailMessage(senderemail, receiveremail))
            {
                mess.Subject = "Password Reset";
                mess.Body = body;
                mess.IsBodyHtml = true;
                
                smtp.Send(mess);
            }

            ViewBag.Result = "Password reset instructions have been sent to your email.";
        }
        else
        {
            ViewBag.Result = "No user found with this email.";
        }

        db.con.Close();
    }

    return View();
}

            public ActionResult ResetPassword(string userId, string token)
{
   db.con.Open();
string query = "SELECT * FROM Users WHERE User_Id = @UserId AND ResetPasswordToken = @Token AND ResetPasswordTokenExpiry > @CurrentTime";
SqlCommand cmd = new SqlCommand(query, db.con);
cmd.Parameters.AddWithValue("@UserId", userId);
cmd.Parameters.AddWithValue("@Token", token);
cmd.Parameters.AddWithValue("@CurrentTime", DateTime.Now);

SqlDataReader reader = cmd.ExecuteReader();
Users user = null;

if (reader.Read())
{
    user = new Users
    {
        // Populate user properties as needed
        // For example: Id = Convert.ToInt32(reader["User_Id"]),
    };
}

reader.Close();
db.con.Close();

if (user == null)
{
    // Token is invalid or expired, handle accordingly (e.g., show an error message)
    ViewBag.Result = "Invalid or expired token.";
    return View(); // You need to create an Error view with appropriate content
}

// If the token is valid, allow the user to reset their password
return View();
}

            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult ResetPassword(string userId, string token, string newPassword,string ConfirmPassword)
{
    // Check if the token is valid and get the user details
    db.con.Open();
    string query = "SELECT * FROM Users WHERE User_Id = @UserId AND ResetPasswordToken = @Token AND ResetPasswordTokenExpiry > @CurrentTime";
    SqlCommand cmd = new SqlCommand(query, db.con);
    cmd.Parameters.AddWithValue("@UserId", userId);
    cmd.Parameters.AddWithValue("@Token", token);
    cmd.Parameters.AddWithValue("@CurrentTime", DateTime.Now);

    SqlDataReader reader = cmd.ExecuteReader();
    Users user = null;

    if (reader.Read())
    {
        user = new Users
        {
            // Populate user properties as needed
            // For example: Id = Convert.ToInt32(reader["User_Id"]),
        };
    }

    reader.Close();



    if (user == null)
    {
        db.con.Close();
        // Token is invalid or expired, handle accordingly (e.g., show an error message)
        ViewBag.Result = "Invalid or expired token.";
        return View(); // You need to create an Error view with appropriate content
    }

    if (newPassword != ConfirmPassword)
    {
        ViewBag.Result2 = "The password does not match the confirm password.";
        return View();
    }


    if (!IsPasswordValid(newPassword))
    {
        db.con.Close();
        ViewBag.Result2 = "Your password must be at least 8 characters long and include at least one uppercase letter and one special character.";
        return View(); // Show ResetPassword view again with the error message
    }

   
    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

    // Reset the user's password
    string updateQuery = "UPDATE Users SET Password = @Password, ResetPasswordToken = NULL, ResetPasswordTokenExpiry = NULL WHERE User_Id = @UserId";
    SqlCommand updateCmd = new SqlCommand(updateQuery, db.con);
    updateCmd.Parameters.AddWithValue("@Password", hashedPassword);
    updateCmd.Parameters.AddWithValue("@UserId", userId);
    updateCmd.ExecuteNonQuery();

    db.con.Close();
 
    ViewBag.Result = "Password has been reset successfully.";
    return View(); // Redirect the user to the login page or any other appropriate page
}



        public ActionResult LogOut()
        {
            Session.RemoveAll(); //Clear all session variables
            return RedirectToAction("SignIn", "Accounts");
        }


        private bool IsPasswordValid(string password)
        {
            // Minimum length check
            if (password.Length < 8)
                return false;

            // Check for at least one uppercase letter
            if (!password.Any(char.IsUpper))
                return false;

            // Check for at least one special character
            if (!password.Any(IsSpecialCharacter))
                return false;

            return true;
        }

        private bool IsSpecialCharacter(char c)
        {
            // Define your set of special characters here
            string specialCharacters = "!@#$%^&*()-_=+[]{}|;:'\",.<>/?";

            // Check if the character is a special character
            return specialCharacters.Contains(c);
        }




    }
}
