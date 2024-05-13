using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventorySystem.Models
{
    public class StockIn
    {
        public int Stock_id { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public int Weight { get; set; }
        public int RetailPrice { get; set; }
        public int PurchasingPrice { get; set; }
        public string ExpiryDate { get; set; }
        public DateTime EntryDate { get; set; }
        public int User_Id { get; set; }
        public string Email { get; set; }
        public int AddQuantity { get; set; }
        public int SellingPrice { get; set; }
        public int Cart_id { get; set; }
        public string CartStatus { get; set; }
        public string SoldTo { get; set; }
        public int Sold_id { get; set; }
        public DateTime SoldDate {get; set; }
        public int Investment { get; set; }
        public int Revenue { get; set; }
        public int Profit { get; set; }
        public int TotalStock { get; set; }
        public int SoldQuantity { get; set; }
        public int StockInQuantity { get; set; }
        public int StockInHand { get; set; }


    }
}