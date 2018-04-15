using Atentis.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtentisPrimer
{
    public static class InfoDataIn
    {
        public static Slot data_slot;  // Информационный слот
        public static Slot trans_slot; // Транзакционный слот
        public static string Server = "213.181.16.52";
        public static string Login = "forts893";
        public static string Password = "VWQWXPW";
        public static int Port = 7800;

        public static string SecBoard_Si = "Si";
        public static string SecCode_Si = "Si-6.18";

        public static string SecBoard_Ri = "RTS";
        public static string SecCode_Ri = "RTS-6.18";

        public static bool RobotWork = false;
        public static DataTable OrderBook_Si = null;
        public static DataTable OrderBook_Ri = null;
        public static DataTable Orders = null;

        public static DataOrderBook[] bq_Si = { new DataOrderBook() { Price = 0, Quantity = 0 } };
        public static DataOrderBook[] sq_Si = { new DataOrderBook() { Price = 0, Quantity = 0 } };

        public static DataOrderBook[] bq_Ri = { new DataOrderBook() { Price = 0, Quantity = 0 } };
        public static DataOrderBook[] sq_Ri = { new DataOrderBook() { Price = 0, Quantity = 0 } };
        public static Table table_Sec;
        public static Table Si_OrderBook;
        public static Table Ri_OrderBook;
        public static Table table_Orders;

        public static string ACCOUNT, BROKERREF, SERVERTYPE;

        public static IList<double> ARRAY = new List<double>();



    }
}
