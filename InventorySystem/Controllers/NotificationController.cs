using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InventorySystem.Models;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;

namespace InventorySystem.Controllers
{
    public class NotificationController : Controller
    {
        //
        // GET: /Notification/

        DataBase db = new DataBase();

        public ActionResult Index()
        {
            return View();
        }




        public ActionResult SendEmail(StockIn si, string subject)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DateTime lastSentTime = GetLastSentTimeFromStorage(); // Implement this method to retrieve the last sent time
                    TimeSpan timeSinceLastSent = DateTime.Now - lastSentTime;
                    int hoursSinceLastSent = (int)timeSinceLastSent.TotalHours;
                    if (hoursSinceLastSent >= 24)
                    {
                        List<StockIn> files = new List<StockIn>();

                        db.con.Open();
                        string q1 = "SELECT * FROM StockIn";
                        SqlCommand cmd1 = new SqlCommand(q1, db.con);
                        SqlDataReader sdr = cmd1.ExecuteReader();

                        while (sdr.Read())
                        {
                            files.Add(new StockIn
                            {
                                ExpiryDate = sdr["ExpiryDate"].ToString(),
                                Name = sdr["Name"].ToString(),
                                Quantity = int.Parse(sdr["Quantity"].ToString()),
                                // Add other properties as needed
                            });
                        }

                        db.con.Close();
                        sdr.Close();

                        // Define a threshold for how many days ahead you want to consider as "near"
                        int thresholdDays = 7;

                        foreach (var item in files)
                        {
                            // Calculate the difference in days between the current date and the expiry date
                            DateTime expiryDate = Convert.ToDateTime(item.ExpiryDate);
                            int daysRemaining = (expiryDate - DateTime.Now).Days;

                            // Check if the expiry date is within the specified threshold
                            if (daysRemaining <= thresholdDays && daysRemaining >= 0)
                            {
                                // If the product is near expiry, send email notification
                                List<StockIn> adminList = new List<StockIn>();

                                db.con.Open();
                                string q2 = "SELECT * FROM Users WHERE roles IN (@role1, @role2)";
                                SqlCommand cmd2 = new SqlCommand(q2, db.con);
                                cmd2.Parameters.AddWithValue("@role1", "Admin");
                                cmd2.Parameters.AddWithValue("@role2", "SuperAdmin");
                                SqlDataReader sdr2 = cmd2.ExecuteReader();

                                while (sdr2.Read())
                                {
                                    adminList.Add(new StockIn
                                    {
                                        Email = sdr2["Email"].ToString(),
                                        // Add other properties as needed
                                    });
                                }
                                db.con.Close();
                                sdr2.Close();

                                // Send email notification to admins
                                var senderemail = new MailAddress("demo7899999@gmail.com", "Notification");
                                var password = "bfapmnypsntylcde";

                                foreach (var admin in adminList)
                                {
                                    var receiveremail = new MailAddress(admin.Email, "Admin");
                                    // Build the email body using the fetched data
                                    string body = "Product That near Expiry\n" + "Product Name:" + "" + item.Name + "\n" + "Quantity of the product:" + "" + item.Quantity;

                                    var smtp = new SmtpClient
                                    {
                                        Host = "smtp.gmail.com",
                                        Port = 587,
                                        EnableSsl = true,
                                        DeliveryMethod = SmtpDeliveryMethod.Network,
                                        UseDefaultCredentials = false,
                                        Credentials = new NetworkCredential(senderemail.Address, password)
                                    };

                                    using (var mess = new MailMessage(senderemail, receiveremail)
                                    {
                                        Subject = "Expiry Date Notification",
                                        Body = body
                                    })
                                    {
                                        smtp.Send(mess);
                                    }
                                }
                            }
                        }

                        return RedirectToAction("SignIn", "Accounts");
                    }
                    else
                    {
                        // It has not been 24 hours since the last email, so don't send another email
                        // You may want to log this or handle it differently based on your requirements
                        return RedirectToAction("SignIn", "Accounts");
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "There are some problems in sending email or fetching data from the database.";
            }

            return RedirectToAction("SignIn", "Accounts");
        }


       private DateTime GetLastSentTimeFromStorage()
{
    // Return the last sent time stored in your application or system
    // For example, you can store it in a static variable or a file
    // For demonstration, let's assume it's stored in a static variable
    return LastSentTimeTracker.LastSentTime;
}

private void UpdateLastSentTimeInStorage()
{
    // Update the last sent time using the current system time
    // For demonstration, let's update a static variable
    LastSentTimeTracker.LastSentTime = DateTime.Now;
}

// Create a class to track the last sent time
public static class LastSentTimeTracker
{
    // Field to store the last sent time
    private static DateTime _lastSentTime = DateTime.MinValue;

    // Property to access the last sent time
    public static DateTime LastSentTime 
    {
        get { return _lastSentTime; }
        set { _lastSentTime = value; }
    }
}





        public ActionResult ShowNotification(StockIn si)
        {
        // Initialize a list to hold notifications
        List<NotificationViewModel> notifications = new List<NotificationViewModel>();

       
            // Open connection to the database
            
                db.con.Open();

                // Query to fetch notifications from the database
                string query = "SELECT Name, ExpiryDate,Quantity FROM StockIn WHERE DATEDIFF(day, GETDATE(), ExpiryDate) <= 7 AND DATEDIFF(day, GETDATE(), ExpiryDate) >= 0";

                // Execute the query
                using (SqlCommand command = new SqlCommand(query, db.con))
                {
                    // Read data
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Create a NotificationViewModel object for each notification
                            NotificationViewModel notification = new NotificationViewModel
                            {
                                Name = reader["Name"].ToString(),
                                ExpiryDate = reader["ExpiryDate"].ToString(),
                                Quantity = int.Parse(reader["Quantity"].ToString()),
                            };

                            // Add the notification to the list
                            notifications.Add(notification);
                        }
                    }
                }

                // Close connection
                db.con.Close();
            
        
      
        // Return the list of notifications as JSON
        return Json(notifications, JsonRequestBehavior.AllowGet);
    }
        



                    }

             }