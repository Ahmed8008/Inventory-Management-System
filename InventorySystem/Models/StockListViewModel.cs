using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventorySystem.Models
{
    public class StockListViewModel
    {
        public List<StockIn> AvailableStock { get; set; }
        public List<StockIn> SoldStock { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int StockInHand { get; set; }
        public int TotalInvestment { get; set; }
        public int TotalRevenue { get; set; }
        public int TotalProfit { get; set; }
        public int PerviousProfit { get; set; }
        public int OverAllProfit { get; set; }

    }
}