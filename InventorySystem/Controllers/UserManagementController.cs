using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InventorySystem.Models;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using BCrypt.Net;
namespace InventorySystem.Controllers
{
    public class UserManagementController : Controller
    {
        //
        // GET: /UserManagement/

        DataBase db = new DataBase();

        public ActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UserCreation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserCreation(Users u)
        {
            if (Session["Email"] != null)
            {
                db.con.Open();

                if (ModelState.IsValid)
                {
                    string checkEmailQuery = "SELECT COUNT(*) FROM [Users] WHERE Email = @Email";
                    SqlCommand checkEmailCmd = new SqlCommand(checkEmailQuery, db.con);
                    checkEmailCmd.Parameters.AddWithValue("@Email", u.Email);
                    int existingUserCount = Convert.ToInt32(checkEmailCmd.ExecuteScalar());

                    if (existingUserCount > 0)
                    {
                        ViewBag.ErrorMessage = "this email already exists";
                        return View();
                    }
                    else
                    {

                        string q = "insert into [Users] (Name,Email,Password,roles,ResetPasswordToken,ResetPasswordTokenExpiry,UserStatus) values(@Name,@Email,@Password,@roles,@ResetPasswordToken,@ResetPasswordTokenExpiry,@UserStatus)";
                        SqlCommand cmd = new SqlCommand(q, db.con);
                        cmd.Parameters.AddWithValue("@Name", u.Name);
                        cmd.Parameters.AddWithValue("@Email", u.Email);
                        cmd.Parameters.AddWithValue("@Password", "None");
                        cmd.Parameters.AddWithValue("@roles", "None");
                        cmd.Parameters.AddWithValue("@ResetPasswordToken", "None");
                        cmd.Parameters.AddWithValue("@ResetPasswordTokenExpiry", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UserStatus", "Waiting to be verified");
                        cmd.ExecuteNonQuery();
                        db.con.Close();
                       

                        db.con.Open();
                        string query = "SELECT * FROM Users WHERE Email = @Email";
                        SqlCommand cmd1 = new SqlCommand(query, db.con);
                        cmd1.Parameters.AddWithValue("@Email", u.Email);

                        SqlDataReader reader = cmd1.ExecuteReader();

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
                            var senderemail = new MailAddress("demo7899999@gmail.com", "VerifyEmail");
                            var password = "bfapmnypsntylcde";

                            var receiveremail = new MailAddress(u.Email, "Receiver");

                            // Build the email body with the password reset link
                            var callbackUrl = Url.Action("SetPassword", "UserManagement", new { userId = user.User_Id, token = token }, Request.Url.Scheme);
                            string body = "Please verify your email by clicking <a href='" + callbackUrl + "'>here</a>";

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
                                mess.Subject = "Verify Your Email";
                                mess.Body = body;
                                mess.IsBodyHtml = true;

                                smtp.Send(mess);
                            }
                        }
                        ViewBag.Result = "Account Created Successfully and waiting to be verified";
                        return View();
                    }

                }

            }
            return View();
        }


        public ActionResult SetPassword(string userId, string token)
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
        public ActionResult SetPassword(string userId, string token, string newPassword,string ConfirmPassword)
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
            string updateQuery = "UPDATE Users SET Password = @Password, ResetPasswordToken = NULL, ResetPasswordTokenExpiry = NULL,UserStatus=@UserStatus WHERE User_Id = @UserId";
            SqlCommand updateCmd = new SqlCommand(updateQuery, db.con);
            updateCmd.Parameters.AddWithValue("@Password", hashedPassword);
            updateCmd.Parameters.AddWithValue("@UserId", userId);
            updateCmd.Parameters.AddWithValue("@UserStatus", "User Verified");

            updateCmd.ExecuteNonQuery();

            db.con.Close();

            ViewBag.Result = "Password has been Set successfully.";
            return View(); // Redirect the user to the login page or any other appropriate page
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


        public ActionResult UsersList()
        {
            if (Session["Email"] != null)
            {
                List<Users> verifiedUsers = new List<Users>();
                List<Users> unverifiedUsers = new List<Users>();

                db.con.Open();

                string qVerified = "SELECT * FROM Users WHERE roles <> 'SuperAdmin' and UserStatus = @UserStatusVerified";
                SqlCommand cmdVerified = new SqlCommand(qVerified, db.con);
                cmdVerified.Parameters.AddWithValue("@UserStatusVerified", "User Verified");
                SqlDataReader sdrVerified = cmdVerified.ExecuteReader();

                while (sdrVerified.Read())
                {
                    verifiedUsers.Add(new Users
                    {
                        User_Id = int.Parse(sdrVerified["User_Id"].ToString()),
                        Name = sdrVerified["Name"].ToString(),
                        Email = sdrVerified["Email"].ToString(),
                        UserStatus = sdrVerified["UserStatus"].ToString(),
                        roles = sdrVerified["roles"].ToString()
                    });
                }
                sdrVerified.Close();

                string qUnverified = "SELECT * FROM Users WHERE roles <> 'SuperAdmin' and UserStatus = @UserStatusUnverified";
                SqlCommand cmdUnverified = new SqlCommand(qUnverified, db.con);
                cmdUnverified.Parameters.AddWithValue("@UserStatusUnverified", "Waiting to be verified");
                SqlDataReader sdrUnverified = cmdUnverified.ExecuteReader();

                while (sdrUnverified.Read())
                {
                    unverifiedUsers.Add(new Users
                    {
                        User_Id = int.Parse(sdrUnverified["User_Id"].ToString()),
                        Name = sdrUnverified["Name"].ToString(),
                        Email = sdrUnverified["Email"].ToString(),
                        UserStatus = sdrUnverified["UserStatus"].ToString(),
                        roles = sdrUnverified["roles"].ToString()
                    });
                }
                sdrUnverified.Close();

                db.con.Close();

                ViewBag.VerifiedUsers = verifiedUsers;
                ViewBag.UnverifiedUsers = unverifiedUsers;

                return View();
            }
            else
            {
                return RedirectToAction("SignIn", "Accounts");
            }
        }


        public ActionResult UpdateRole(int id, Users u)
        {
            db.con.Open();

            string q = "Update Users set roles=(@roles) where User_Id=(@User_Id) ";
            SqlCommand cmd = new SqlCommand(q, db.con);
            cmd.Parameters.AddWithValue("@roles", u.roles);
            cmd.Parameters.AddWithValue("@User_Id", id);
            cmd.ExecuteNonQuery();
            db.con.Close();
            return RedirectToAction("UsersList");
        }



        public ActionResult RemoveUser(int id)
        {
            db.con.Open();
            string q2 = "delete from Users where User_Id=(@User_Id)";
            SqlCommand cmdd = new SqlCommand(q2, db.con);
            cmdd.Parameters.AddWithValue("@User_Id", id);
            cmdd.ExecuteNonQuery();
            db.con.Close();
            return RedirectToAction("UsersList");
        }
    
    }
}
