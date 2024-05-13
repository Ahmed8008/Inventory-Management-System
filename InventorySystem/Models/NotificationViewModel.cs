using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InventorySystem.Models
{
    public class NotificationViewModel
    {
        public string Name { get; set; }
        public String ExpiryDate { get; set; }
        public int Quantity { get; set; }

    }
}