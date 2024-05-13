using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InventorySystem.Models;
using System.Data.SqlClient;

namespace InventorySystem.Controllers
{
    public class FilterController : Controller
    {
        //
        // GET: /Filter/

        DataBase db = new DataBase();

        public ActionResult FilterList(string searchName = "", string searchQuantity = "", string searchWeight = "", string searchRetailPrice = "", string searchPurchasingPrice = "", string searchExpiryDate = "", string searchEntryDate = "")
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



                ViewBag.CurrentSearch = searchName;
                ViewBag.CurrentSearch = searchQuantity;
                ViewBag.CurrentSearch = searchWeight;
                ViewBag.CurrentSearch = searchRetailPrice;
                ViewBag.CurrentSearch = searchExpiryDate;
                ViewBag.CurrentSearch = searchEntryDate;




                if (!string.IsNullOrEmpty(searchName) || !string.IsNullOrEmpty(searchQuantity) || !string.IsNullOrEmpty(searchWeight) || !string.IsNullOrEmpty(searchRetailPrice) || !string.IsNullOrEmpty(searchPurchasingPrice) || !string.IsNullOrEmpty(searchExpiryDate) || !string.IsNullOrEmpty(searchEntryDate))
                {
                    string q = "SELECT * FROM StockIn WHERE " +
                   "Name LIKE @searchName OR " +
                   "Quantity LIKE @searchQuantity OR " +
                   "Weight LIKE @searchWeight OR " +
                   "RetailPrice LIKE @searchRetailPrice OR " +
                   "PurchasingPrice LIKE @searchPurchasingPrice OR " +
                   "ExpiryDate LIKE @searchExpiryDate OR " +
                   "EntryDate LIKE @searchEntryDate";

                    SqlCommand cmd = new SqlCommand(q, db.con);
                    cmd.Parameters.AddWithValue("@searchName", "%" + searchName + "%");
                    cmd.Parameters.AddWithValue("@searchQuantity", "%" + searchQuantity + "%");
                    cmd.Parameters.AddWithValue("@searchWeight", "%" + searchWeight + "%");
                    cmd.Parameters.AddWithValue("@searchRetailPrice", "%" + searchRetailPrice + "%");
                    cmd.Parameters.AddWithValue("@searchPurchasingPrice", "%" + searchPurchasingPrice + "%");
                    cmd.Parameters.AddWithValue("@searchExpiryDate", "%" + searchExpiryDate + "%");
                    cmd.Parameters.AddWithValue("@searchEntryDate", "%" + searchEntryDate + "%");


                    SqlDataReader sdr = cmd.ExecuteReader();

                    while (sdr.Read())
                    {
                        availableStock.Add(new StockIn
                        {
                            Stock_id = int.Parse(sdr["Stock_id"].ToString()),
                            Quantity = int.Parse(sdr["Quantity"].ToString()),
                            Name = sdr["Name"].ToString(),
                            Weight = int.Parse(sdr["Weight"].ToString()),
                            RetailPrice = int.Parse(sdr["RetailPrice"].ToString()),
                            PurchasingPrice = int.Parse(sdr["PurchasingPrice"].ToString()),
                            ExpiryDate = sdr["ExpiryDate"].ToString(),
                            EntryDate = Convert.ToDateTime(sdr["EntryDate"]),
                            AddQuantity = int.Parse(sdr["AddQuantity"].ToString()),
                        });
                    }

                    sdr.Close();
                }
                else
                {
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
                }

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
            return RedirectToAction("FilterList");
        }


    }
}
