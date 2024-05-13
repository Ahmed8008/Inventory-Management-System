using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace InventorySystem.Models
{
    public class DataBase
    {
        public static string constr = @"workstation id=InventorySystemm.mssql.somee.com;packet size=4096;user id=ahmed69_SQLLogin_1;pwd=wlq66phuan;data source=InventorySystemm.mssql.somee.com;persist security info=False;initial catalog=InventorySystemm;TrustServerCertificate=True";

        public SqlConnection con = new SqlConnection(constr);
    }
}