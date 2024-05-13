using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InventorySystem.Models;
using System.Data.SqlClient;

namespace InventorySystem.Controllers
{
    public class StockController : Controller
    {
        //
        // GET: /Stock/

        DataBase db = new DataBase();

        [HttpGet]
        public ActionResult StockIn()
        {
            return View();
        }


        [HttpPost]
        public ActionResult StockIn(StockIn si)
        {
            if (Session["Email"] != null)
            {
                db.con.Open();


                int previousProfit = 0;
                string fetchLastProfitQuery = "SELECT TOP 1 Profit FROM Inquiry ORDER BY Inquiry_id DESC";
                using (SqlCommand fetchLastProfitCmd = new SqlCommand(fetchLastProfitQuery, db.con))
                {
                    object lastProfitObj = fetchLastProfitCmd.ExecuteScalar();
                    if (lastProfitObj != null && lastProfitObj != DBNull.Value)
                    {
                        previousProfit = Convert.ToInt32(lastProfitObj);
                    }
                }


                // Step 1: Retrieve the previous profit
                

                // Step 2: Calculate investment and revenue
                int investment = si.Quantity * si.PurchasingPrice;
                int revenue = si.Revenue;

                // Step 3: Calculate new profit
                int newProfit = previousProfit + (revenue - investment);

                // Step 4: Update the previous profit in the database
               

                // Insert new data into StockIn and Inquiry tables
                string insertQuery = "INSERT INTO [StockIn] (Quantity, Name, Weight, RetailPrice, PurchasingPrice, ExpiryDate, EntryDate, User_Id, AddQuantity) VALUES (@Quantity, @Name, @Weight, @RetailPrice, @PurchasingPrice, @ExpiryDate, @EntryDate, @User_Id, @AddQuantity); " +
                                    "INSERT INTO [Inquiry] (Quantity, Name, EntryDate, RetailPrice, PurchasingPrice, ExpiryDate, SellingPrice, Investment, Revenue, Profit, User_Id) VALUES (@Quantity, @Name, @EntryDate, @RetailPrice, @PurchasingPrice, @ExpiryDate, @SellingPrice, @Investment, @Revenue, @NewProfit, @User_Id)";

                SqlCommand cmd = new SqlCommand(insertQuery, db.con);
                cmd.Parameters.AddWithValue("@Quantity", si.Quantity);
                cmd.Parameters.AddWithValue("@Name", si.Name);
                cmd.Parameters.AddWithValue("@Weight", si.Weight);
                cmd.Parameters.AddWithValue("@RetailPrice", si.RetailPrice);
                cmd.Parameters.AddWithValue("@PurchasingPrice", si.PurchasingPrice);
                cmd.Parameters.AddWithValue("@ExpiryDate", si.ExpiryDate);
                cmd.Parameters.AddWithValue("@EntryDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@User_Id", Session["User_Id"]);
                cmd.Parameters.AddWithValue("@AddQuantity", si.AddQuantity);

                // Assuming values for the Inquiry table (replace with actual values as needed)
                cmd.Parameters.AddWithValue("@SellingPrice", si.SellingPrice);
                cmd.Parameters.AddWithValue("@Investment", investment);
                cmd.Parameters.AddWithValue("@Revenue", revenue);
                cmd.Parameters.AddWithValue("@NewProfit", newProfit); // Add this line to declare @NewProfit parameter

                // Execute the SQL command
                cmd.ExecuteNonQuery();
                previousProfit = newProfit;
                db.con.Close();
                ViewBag.Result = "Stock Added Successfully";
                return View();
            }
            else
            {
                return RedirectToAction("SignIn", "Accounts");
            }
        }



        public ActionResult StockList()
        {
            if (Session["Email"] != null)
            {
                List<StockIn> availableStock = new List<StockIn>();
                List<string> allNames = new List<string>();
                List<int> allquantity = new List<int>();
                List<int> allweight = new List<int>();
                List<int> allretailprice = new List<int>();
                List<int> allpurchasingprice = new List<int>();
                List<DateTime> allexpiryDate = new List<DateTime>();
                List<DateTime> allentryDate = new List<DateTime>();

                db.con.Open();

                // Retrieve all unique names for dropdown
                string qq = "SELECT * FROM StockIn";
                SqlCommand cmdq = new SqlCommand(qq, db.con);
                SqlDataReader sdrq = cmdq.ExecuteReader();

                while (sdrq.Read())
                {
                    allNames.Add(sdrq["Name"].ToString());
                    allquantity.Add(Convert.ToInt32(sdrq["Quantity"]));
                    allweight.Add(Convert.ToInt32(sdrq["Weight"]));
                    allretailprice.Add(Convert.ToInt32(sdrq["RetailPrice"]));
                    allpurchasingprice.Add(Convert.ToInt32(sdrq["PurchasingPrice"]));
                    allexpiryDate.Add(Convert.ToDateTime(sdrq["ExpiryDate"]));
                    allentryDate.Add(Convert.ToDateTime(sdrq["EntryDate"]));

                }

                sdrq.Close();
                ViewBag.AllNames = allNames;
                ViewBag.AllQuantity = allquantity;
                ViewBag.AllWeight = allweight;
                ViewBag.AllRetailPrice = allretailprice;
                ViewBag.AllPurchasingPrice = allpurchasingprice;
                ViewBag.AllExpiryDate = allexpiryDate;
                ViewBag.AllEntryDate = allentryDate;

                // Set the current search parameter for the form action
               
               
                    string q1 = "SELECT * FROM StockIn";
                    SqlCommand cmd1 = new SqlCommand(q1, db.con);

                    SqlDataReader sdr1 = cmd1.ExecuteReader();
                    while (sdr1.Read())
                    {
                        availableStock.Add(new StockIn
                        {
                            Stock_id = int.Parse(sdr1["Stock_id"].ToString()),
                            Quantity = int.Parse(sdr1["Quantity"].ToString()),
                            Name = sdr1["Name"].ToString(),
                            Weight = int.Parse(sdr1["Weight"].ToString()),
                            RetailPrice = int.Parse(sdr1["RetailPrice"].ToString()),
                            PurchasingPrice = int.Parse(sdr1["PurchasingPrice"].ToString()),
                            ExpiryDate = sdr1["ExpiryDate"].ToString(),
                            EntryDate = Convert.ToDateTime(sdr1["EntryDate"]),
                            AddQuantity = int.Parse(sdr1["AddQuantity"].ToString()),
                        });
                    }

                    sdr1.Close();
                

                db.con.Close();
                return View(availableStock);
            }
            else
            {
                return RedirectToAction("SignIn", "Accounts");
            }
        }

        
        public ActionResult AddQuantity(int id, StockIn si)
        {
            // Fetch the current StockIn record from the database
            db.con.Open();
            string selectQuery = "SELECT * FROM StockIn WHERE Stock_id = @Stock_id";
            SqlCommand selectCmd = new SqlCommand(selectQuery, db.con);
            selectCmd.Parameters.AddWithValue("@Stock_id", id);
            SqlDataReader selectReader = selectCmd.ExecuteReader();

            if (selectReader.Read())
            {
                int currentQuantity = int.Parse(selectReader["Quantity"].ToString());
                int addQuantity = si.Quantity;

                if (addQuantity <= 0 || addQuantity > currentQuantity)
                {
                    TempData["ErrorMessage"] = "Error: Add quantity cannot be less than current quantity";
                }

                else
                {
                    // Calculate the new total quantity
                    int newTotalQuantity = currentQuantity - addQuantity;

                    // Close the reader before executing the update command
                    selectReader.Close();

                    // Update the Quantity in the StockIn table
                    string updateQuery = "UPDATE StockIn SET Quantity = @NewQuantity WHERE Stock_id = @Stock_id";
                    SqlCommand updateCmd = new SqlCommand(updateQuery, db.con);
                    updateCmd.Parameters.AddWithValue("@NewQuantity", newTotalQuantity);
                    updateCmd.Parameters.AddWithValue("@Stock_id", id);
                    updateCmd.ExecuteNonQuery();

                    // Insert into AddCart table
                    string insertQuery = "INSERT INTO AddCart (Stock_id, SellingPrice, User_Id, Quantity) VALUES (@Stock_id, @SellingPrice, @User_Id, @Quantity)";
                    SqlCommand insertCmd = new SqlCommand(insertQuery, db.con);
                    insertCmd.Parameters.AddWithValue("@Stock_id", id);
                    insertCmd.Parameters.AddWithValue("@SellingPrice", "0");
                    insertCmd.Parameters.AddWithValue("@User_Id", Session["User_Id"]);
                    insertCmd.Parameters.AddWithValue("@Quantity", si.Quantity);

                    insertCmd.ExecuteNonQuery();
                }
            }

            db.con.Close();

            // Retrieve the search parameter from the query string

            // Redirect to StockList with the current search parameter
            return RedirectToAction("StockList");
        }


        public ActionResult UpdateQuantity(int id1,int id2,StockIn si)
        {

            db.con.Open();
            string selectQuery = "SELECT si.Quantity As StockQuantity,ac.Quantity AS CartQuantity FROM AddCart ac inner join StockIn si on ac.Stock_id=si.Stock_id WHERE ac.Stock_id = @Stock_id and ac.Cart_id=@Cart_id";
            SqlCommand selectCmd = new SqlCommand(selectQuery, db.con);
            selectCmd.Parameters.AddWithValue("@Stock_id", id1);
            selectCmd.Parameters.AddWithValue("@Cart_id", id2);

            SqlDataReader selectReader = selectCmd.ExecuteReader();

            if (selectReader.Read())
            {
                int CartQuantity = int.Parse(selectReader["CartQuantity"].ToString());
                int StockQuantity = int.Parse(selectReader["StockQuantity"].ToString());

                int totalQuantity;
                // Calculate the new total quantity

                // Close the reader before executing the update command
                selectReader.Close();

                // Update the Quantity in the StockIn table
                string updateQuery = "UPDATE StockIn SET Quantity = @NewQuantity WHERE Stock_id = @Stock_id";
                SqlCommand updateCmd = new SqlCommand(updateQuery, db.con);
                updateCmd.Parameters.AddWithValue("@NewQuantity", totalQuantity= CartQuantity + StockQuantity);
                updateCmd.Parameters.AddWithValue("@Stock_id", id1);
                updateCmd.ExecuteNonQuery();

                string q2 = "delete from AddCart where Cart_id=(@Cart_id)";
                SqlCommand cmdd = new SqlCommand(q2, db.con);
                cmdd.Parameters.AddWithValue("@Cart_id", id2);
                cmdd.ExecuteNonQuery();

                
            }

            db.con.Close();

            return RedirectToAction("ShowCart");

        }


        public ActionResult ShowCart()
        {
            if (Session["Email"] != null)
            {
                List<StockIn> files = new List<StockIn>();
                db.con.Open();
                string q = "select ac.Cart_id,ac.Quantity,si.Name,si.Weight,si.RetailPrice,si.PurchasingPrice,ac.SellingPrice,si.ExpiryDate,si.EntryDate,si.Stock_id from StockIn si inner Join AddCart ac on si.Stock_id=ac.Stock_id where ac.User_Id=(@User_Id)";
                SqlCommand cmd = new SqlCommand(q, db.con);
                cmd.Parameters.AddWithValue("User_Id", Session["User_Id"]);
                SqlDataReader sdr = cmd.ExecuteReader();

                while (sdr.Read())
                {
                    files.Add(new StockIn
                    {
                        Cart_id = int.Parse(sdr["Cart_id"].ToString()),
                        Stock_id = int.Parse(sdr["Stock_id"].ToString()),
                        Quantity = int.Parse(sdr["Quantity"].ToString()),
                        Name = sdr["Name"].ToString(),
                        Weight = int.Parse(sdr["Weight"].ToString()),
                        RetailPrice = int.Parse(sdr["RetailPrice"].ToString()),
                        PurchasingPrice = int.Parse(sdr["PurchasingPrice"].ToString()),
                        SellingPrice = int.Parse(sdr["SellingPrice"].ToString()),
                        ExpiryDate = sdr["ExpiryDate"].ToString(),
                        EntryDate = Convert.ToDateTime(sdr["EntryDate"]),

                    });
                }
                db.con.Close();
                sdr.Close();
                return View(files);
            }

            else
            {
                return RedirectToAction("SignIn", "Accounts");

            }
        }

        public ActionResult UpdateSellingPrice(int id, StockIn si)
        {
            db.con.Open();
            string q = "Update [AddCart] set SellingPrice =(@SellingPrice) where Cart_id='" + id + "'";
            SqlCommand cmd = new SqlCommand(q, db.con);
            cmd.Parameters.AddWithValue("@SellingPrice", si.SellingPrice);
            cmd.ExecuteNonQuery();
            db.con.Close();
            return RedirectToAction("ShowCart");
        }


        public ActionResult SoldStock(StockIn si, int id, string SoldTo)
        {
           
                db.con.Open();

                List<StockIn> files = new List<StockIn>();

                // Use parameterized query to prevent SQL injection
                using (SqlCommand cmd = new SqlCommand("SELECT ac.Cart_id, ac.Quantity, si.Quantity As StockInQuantity , si.AddQuantity, si.Name, si.Weight, si.RetailPrice, si.PurchasingPrice, ac.SellingPrice, si.ExpiryDate, si.EntryDate, si.Stock_id FROM StockIn si INNER JOIN AddCart ac ON si.Stock_id = ac.Stock_id", db.con))
                {
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            files.Add(new StockIn
                            {
                                Cart_id = Convert.ToInt32(sdr["Cart_id"]),
                                Stock_id = Convert.ToInt32(sdr["Stock_id"]),
                                SellingPrice = Convert.ToInt32(sdr["SellingPrice"]),
                                Quantity = Convert.ToInt32(sdr["Quantity"]),
                                Name = sdr["Name"].ToString(),
                                ExpiryDate = sdr["ExpiryDate"].ToString(),
                                StockInQuantity = Convert.ToInt32(sdr["StockInQuantity"]),
                            });
                        }
                    }
                }

               
                int previousProfit = 0;
                string fetchLastProfitQuery = "SELECT TOP 1 Profit FROM Inquiry ORDER BY Inquiry_id DESC";
                using (SqlCommand fetchLastProfitCmd = new SqlCommand(fetchLastProfitQuery, db.con))
                {
                    object lastProfitObj = fetchLastProfitCmd.ExecuteScalar();
                    if (lastProfitObj != null && lastProfitObj != DBNull.Value)
                    {
                        previousProfit = Convert.ToInt32(lastProfitObj);
                    }
                }

                foreach (var item in files)
                {
                    // Step 2: Calculate revenue and investment
                    int investment = 0; // Assuming investment is 0 for now
                    int revenue = item.Quantity * item.SellingPrice;

                    // Step 3: Calculate new profit
                    int newProfit = previousProfit + (revenue - investment);

                    // Step 4: Update the profit column in the Inquiry table


                    string q = "INSERT INTO SoldStock (SoldTo, SoldDate, Cart_id, User_Id, Stock_id, SellingPrice, Quantity) VALUES (@SoldTo, @SoldDate, @Cart_id, @User_Id, @Stock_id, @SellingPrice, @Quantity);" +
                                "INSERT INTO [Inquiry] (Quantity, Name, EntryDate, RetailPrice, PurchasingPrice, ExpiryDate, SellingPrice, Investment, Revenue, Profit, User_Id,Stock_id) VALUES (@Quantity, @Name, @EntryDate, @RetailPrice, @PurchasingPrice, @ExpiryDate, @SellingPrice, @Investment, @Revenue, @NewProfit, @User_Id,@Stock_id)";

                    using (SqlCommand cmd1 = new SqlCommand(q, db.con))
                    {
                        cmd1.Parameters.AddWithValue("@SoldTo", SoldTo);
                        cmd1.Parameters.AddWithValue("@SoldDate", DateTime.Now);
                        cmd1.Parameters.AddWithValue("@Cart_id", item.Cart_id);
                        cmd1.Parameters.AddWithValue("@Stock_id", item.Stock_id);
                        cmd1.Parameters.AddWithValue("@Quantity", item.Quantity);
                        cmd1.Parameters.AddWithValue("@Name", item.Name);
                        cmd1.Parameters.AddWithValue("@EntryDate", DateTime.Now);
                        cmd1.Parameters.AddWithValue("@RetailPrice", 0);
                        cmd1.Parameters.AddWithValue("@PurchasingPrice", 0);
                        cmd1.Parameters.AddWithValue("@ExpiryDate", item.ExpiryDate);
                        cmd1.Parameters.AddWithValue("@SellingPrice", item.SellingPrice);
                        cmd1.Parameters.AddWithValue("@Investment", 0);
                        cmd1.Parameters.AddWithValue("@Revenue", revenue);
                        cmd1.Parameters.AddWithValue("@NewProfit", newProfit);
                        cmd1.Parameters.AddWithValue("@User_Id", id);

                        cmd1.ExecuteNonQuery();

                        // Update previousProfit for next iteration
                        previousProfit = newProfit;
                    }
                }

                // Delete sold items from AddCart
                string q2 = "DELETE FROM AddCart WHERE User_Id = @User_Id";
                using (SqlCommand cmdd = new SqlCommand(q2, db.con))
                {
                    cmdd.Parameters.AddWithValue("@User_Id", id);
                    cmdd.ExecuteNonQuery();
                }

            

                return RedirectToAction("ShowCart");
            
           
        }




        public ActionResult SoldList()
        {
            if (Session["Email"] != null)
            {
                List<StockIn> files = new List<StockIn>();
                db.con.Open();
                string q = "SELECT ss.Cart_id, ss.Quantity, si.Name, si.Weight, si.RetailPrice, si.PurchasingPrice, ss.SellingPrice, si.ExpiryDate, ss.SoldDate, si.Stock_id,ss.SoldTo FROM StockIn si INNER JOIN SoldStock ss ON si.Stock_id = ss.Stock_id";
                SqlCommand cmd = new SqlCommand(q, db.con);
                SqlDataReader sdr1 = cmd.ExecuteReader();
                while (sdr1.Read())
                {
                    files.Add(new StockIn
                    {
                        Cart_id = int.Parse(sdr1["Cart_id"].ToString()),
                        Quantity = int.Parse(sdr1["Quantity"].ToString()),
                        Name = sdr1["Name"].ToString(),
                        Weight = int.Parse(sdr1["Weight"].ToString()),
                        RetailPrice = int.Parse(sdr1["RetailPrice"].ToString()),
                        PurchasingPrice = int.Parse(sdr1["PurchasingPrice"].ToString()),
                        SellingPrice = int.Parse(sdr1["SellingPrice"].ToString()),
                        ExpiryDate = sdr1["ExpiryDate"].ToString(),
                        SoldDate = Convert.ToDateTime(sdr1["SoldDate"]),
                        Stock_id = int.Parse(sdr1["Stock_id"].ToString()),
                        SoldTo = sdr1["SoldTo"].ToString(),

                    });
                }
                db.con.Close();
                sdr1.Close();
                return View(files);
            }

            else
            {
                return View("HomePage");

            }
        }



        public ActionResult DateInquiry(DateTime? fromDate, DateTime? toDate)
{
    if (fromDate != null && toDate != null)
    {

        db.con.Open();

        string query = "SELECT * from Inquiry WHERE EntryDate >= @FromDate AND EntryDate <= @ToDate";
        SqlCommand cmd = new SqlCommand(query, db.con);
        cmd.Parameters.AddWithValue("@FromDate", fromDate);
        cmd.Parameters.AddWithValue("@ToDate", toDate);

        if (fromDate > toDate)
        {
            db.con.Close();
            return RedirectToAction("InvalidDate");
        }

        List<StockIn> soldStockList = new List<StockIn>();
        int profit = 0;
        using (SqlDataReader reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                int investment = Convert.ToInt32(reader["Investment"]);
                int revenue = Convert.ToInt32(reader["Revenue"]);
                 profit = profit + (revenue - investment);

                // Create a new StockIn object and calculate total profit for each record
                StockIn soldStock = new StockIn
                {
                    Investment = investment,
                    Revenue = revenue,
                    Profit = profit,
                    Name = reader["Name"].ToString(),
                };

                soldStockList.Add(soldStock);
            }
        }

        db.con.Close();

        int totalprofit = profit;
           

            db.con.Open();

            string query2 = "SELECT ISNULL(SUM(Quantity * PurchasingPrice), 0) AS TotalValue FROM StockIn";

            SqlCommand cmd2 = new SqlCommand(query2, db.con);

            int stockInHand = 0; // Variable to store StockInHand value

            using (SqlDataReader reader2 = cmd2.ExecuteReader())
            {
                while (reader2.Read())
                {
                    stockInHand = Convert.ToInt32(reader2["TotalValue"]);
                }
            }

            db.con.Close();

            db.con.Open();

            string investmentQuery = "SELECT ISNULL(SUM(Investment), 0) AS TotalInvestment FROM Inquiry WHERE EntryDate >= @FromDate AND EntryDate <= @ToDate";
            SqlCommand investmentCmd = new SqlCommand(investmentQuery, db.con);
            investmentCmd.Parameters.AddWithValue("@FromDate", fromDate);
            investmentCmd.Parameters.AddWithValue("@ToDate", toDate);
            int totalInvestment = Convert.ToInt32(investmentCmd.ExecuteScalar());

            string revenueQuery = "SELECT ISNULL(SUM(Revenue), 0) AS TotalRevenue FROM Inquiry WHERE EntryDate >= @FromDate AND EntryDate <= @ToDate";
            SqlCommand revenueCmd = new SqlCommand(revenueQuery, db.con);
            revenueCmd.Parameters.AddWithValue("@FromDate", fromDate);
            revenueCmd.Parameters.AddWithValue("@ToDate", toDate);
            int totalRevenue = Convert.ToInt32(revenueCmd.ExecuteScalar());

            db.con.Close();

           

            db.con.Open();

            int perviousProfit = 0;
            string previousProfitQuery = @"SELECT TOP 1 Profit 
                                FROM Inquiry 
                                WHERE Inquiry_id < (SELECT MIN(Inquiry_id) FROM Inquiry WHERE EntryDate >= @FromDate AND EntryDate <= @ToDate)
                                ORDER BY Inquiry_id DESC";

            using (SqlCommand previousProfitCmd = new SqlCommand(previousProfitQuery, db.con))
            {
                previousProfitCmd.Parameters.AddWithValue("@FromDate", fromDate);
                previousProfitCmd.Parameters.AddWithValue("@ToDate", toDate);
                object lastProfitObj = previousProfitCmd.ExecuteScalar();
                if (lastProfitObj != null && lastProfitObj != DBNull.Value)
                {
                    perviousProfit = Convert.ToInt32(lastProfitObj);
                }
            }


            db.con.Close();

//            db.con.Open();

//            int totalProfit = 0;
//            string fetchLastProfitQuery = @"
//    SELECT TOP 1 Profit 
//    FROM Inquiry 
//    WHERE EntryDate >= @FromDate AND EntryDate <= @ToDate 
//    ORDER BY Inquiry_id DESC";

//            using (SqlCommand fetchLastProfitCmd = new SqlCommand(fetchLastProfitQuery, db.con))
//            {
//                fetchLastProfitCmd.Parameters.AddWithValue("@FromDate", fromDate);
//                fetchLastProfitCmd.Parameters.AddWithValue("@ToDate", toDate);

//                object lastProfitObj = fetchLastProfitCmd.ExecuteScalar();
//                if (lastProfitObj != null && lastProfitObj != DBNull.Value)
//                {
//                    totalProfit = Convert.ToInt32(lastProfitObj);
//                }
//            }

//            db.con.Close();

        int overallprofit = perviousProfit + totalprofit;

            // Combine the results from all queries

            // Set the stock in hand value in the view model
            var viewModel = new StockListViewModel
            {
                AvailableStock = soldStockList,
                StockInHand = stockInHand, // Use the variable here
                TotalInvestment = totalInvestment,
                TotalRevenue = totalRevenue,
                OverAllProfit =overallprofit,
                PerviousProfit=perviousProfit
            };

            return View(viewModel);
        
    }

    return View();
}


        public ActionResult InvalidDate()
        {
            return View();
        }



    }
}
