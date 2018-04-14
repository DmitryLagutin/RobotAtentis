using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Data;
using Atentis.Connection;
using System.Threading;
using System.Timers;
using System.IO;
using System.Windows.Forms;

namespace AtentisPrimer
{
    //КЛАСС ТradeHelper
    public static class TradeHelper
    {
        public static PositionStateHandler _del; // Делегат вывода текста в форме

        //окрытие длинной позиции
        public static void OpenLE(this Slot slot, IEnumerable<DataRow> Orders, string account, string brokerRef, string SecBoard, string SecCode, double price, int vol, string comment, string orderLEcache, string NamePos, double ProfitFuture) //Orders = Orders.Select()
        {

            // Вспомогательные переменные ---------
            // account                            - Код торгового счета


            string BuySell = "B";                 // Направление операции: "B" – покупка, "S" – продажа
            string mktLimit = "L";                // Тип заявки: "M" – рыночная, "L" – лимитированная
            string splitFlag = "S";               // Признак расщепления цены: "S" – по разным ценам, "O" – по одной цене
            string immCancel = "";                // Условие исполнения: " " (пробел) – поставить в очередь, "N" – немедленно или отклонить, "W" – снять остаток
            string entryType = "P";               // Тип ввода значения цены: "P" – цена, "Y" – доходность, "W" – средневзвешенная цена
            string marketMaker = "";              // Признак заявки Маркет-Мейкера: " " (пробел) – обычная заявка, "M" – заявка Маркет-Мейкера
            // SecBoard                           -  Код режима торгов для финансового инструмента
            // SecCode                            -  Код финансового инструмента
            string issueCode = "";                // Код выпуска ценной бумаги
            // price                              -  Цена за одну ценную бумагу
            // amount                             -  Количество ценных бумаг, выраженное в лотах
            // hidden (Необязательный параметр)   -  Скрытое количество ценных бумаг, выраженное в лотах
            // brokerRef                          -  Ссылка типа <счет клиента>[/<субсчет>]
            // extRef                             -  Внешняя ссылка (комментарий)
            DateTime endTime = DateTime.MinValue; // Время окончания действия заявки

            //long orderNo;                       // Выходной параметр orderNo содержит номер зарегистрированной заявки
            string resultMsg;                     // Выходной параметр resultMsg содержит текстовое сообщение о результате операции

            //Находим ордер------------------------

            DataRow orderX = null;

            long cache = 0;

            if (orderLEcache.Length > 0)
                cache = long.Parse(orderLEcache, CultureInfo.InvariantCulture);


            foreach (DataRow order in Orders)
            {
                if ((long)order["OrderNo"] == cache)
                    orderX = order;
            }


            //--------------------------------------

            if (orderX == null)
            {
                if (orderLEcache.Length == 0 && price > 0)
                {
                    long orderNo;
                    int result = 0;
                    result = slot.AddOrder(account, BuySell, mktLimit, splitFlag, immCancel, entryType, marketMaker, SecBoard, SecCode, issueCode, price, vol, brokerRef, comment, endTime, out orderNo, out resultMsg);
                    if (result == 0)
                    {
                        NamePos.SaveOrderDB(orderNo.ToString(), string.Empty);
                    }
                    if(_del != null)
                        _del("Cобытие", resultMsg);
                }
            }
            else
            {
                // если заявка активна
                if ((string)orderX["Status"] == "A" || (string)orderX["Status"] == "O")
                {
                    // Если заявка активна, и ее цена не соответсвует новой цене
                    if ((double)orderX["Price"] != price && price != 0)
                    {
                        long orderNo1;
                        long orderNo2;
                        int result = slot.MoveOrders(0, (long)orderX["OrderNo"], price, 0, comment, 0, 0, 0, comment, out orderNo1, out orderNo2, out resultMsg);
                        if (result == 0)
                           NamePos.SaveOrderDB(orderNo1.ToString(), string.Empty);
                        else
                           NamePos.SaveOrderDB(((long)orderX["OrderNo"]).ToString(), string.Empty);

                        if (_del != null)
                           _del("Cобытие", resultMsg);
                    }
                    // Если выставленна заявка с Ценой == 0, но при этом ордер активен и не исполнен
                    else if (price == 0)
                    {
                        int result;
                        result = slot.DeleteOrder((long)orderX["OrderNo"], out resultMsg);
                        if (result == 0)
                            NamePos.SaveOrderDB(string.Empty, string.Empty);

                        if (_del != null)
                            _del("Cобытие", resultMsg);
                    }
                }
                // если заявка отменена
                else if ((string)orderX["Status"] == "W")
                {
                    long orderNo;
                    int result = 0;
                    result = slot.AddOrder(account, BuySell, mktLimit, splitFlag, immCancel, entryType, marketMaker, SecBoard, SecCode, issueCode, price, vol, brokerRef, comment, endTime, out orderNo, out resultMsg);
                    NamePos.SaveOrderDB(orderNo.ToString(), string.Empty);

                    if (_del != null)
                        _del("Cобытие", resultMsg);


                }
                //----------------------------------------------------------------------

                // если заявка исполнена
                else if ((string)orderX["Status"] == "M")
                {
                    Position posLE = new Position()
                    {
                        SecirityPos = ((string)orderX["SecCode"]).Trim(),
                        Order_Open_No = (long)orderX["OrderNo"],
                        IsLong = true,
                        Quantiry = (int)orderX["Quantity"],
                        OpenPrice = (double)orderX["Price"],
                        CurrentPrice = (double)orderX["Price"] + ProfitFuture, //!!!!!!!!
                        Stop = 0,
                        ProfitFuture = ProfitFuture,
                        Comment = "LE$" + DateTime.Now.Ticks.ToString(),
                        IsActive = true,
                        OpenDate = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)
                    };

                    NamePos.SaveOrderDB(string.Empty, string.Empty);  //сохраняем ордер
                    posLE.SavePosDB();                                //сохраняем позиции

                    if (_del != null)
                        _del("Cобытие", "Открыта LE - pos : sec: " + posLE.SecirityPos + " | номер " + posLE.Comment + " | объем " + posLE.Quantiry + " | цена " + posLE.OpenPrice);
                }
            
            }


        }

        //окрытие короткой позиции
        public static void OpenSH(this Slot slot, IEnumerable<DataRow> Orders, string account, string brokerRef, string SecBoard, string SecCode, double price, int vol, string comment, string orderSHcache, string NamePos, double ProfitFuture) //Orders = Orders.Select()
        {
            // Вспомогательные переменные ---------
            // account                            - Код торгового счета
            string BuySell = "S";                 // Направление операции: "B" – покупка, "S" – продажа
            string mktLimit = "L";                // Тип заявки: "M" – рыночная, "L" – лимитированная
            string splitFlag = "S";               // Признак расщепления цены: "S" – по разным ценам, "O" – по одной цене
            string immCancel = "";                // Условие исполнения: " " (пробел) – поставить в очередь, "N" – немедленно или отклонить, "W" – снять остаток
            string entryType = "P";               // Тип ввода значения цены: "P" – цена, "Y" – доходность, "W" – средневзвешенная цена
            string marketMaker = "";              // Признак заявки Маркет-Мейкера: " " (пробел) – обычная заявка, "M" – заявка Маркет-Мейкера
            // SecBoard                           -  Код режима торгов для финансового инструмента
            // SecCode                            -  Код финансового инструмента
            string issueCode = "";                // Код выпуска ценной бумаги
            // price                              -  Цена за одну ценную бумагу
            // amount                             -  Количество ценных бумаг, выраженное в лотах
            // hidden (Необязательный параметр)   -  Скрытое количество ценных бумаг, выраженное в лотах
            // brokerRef                          -  Ссылка типа <счет клиента>[/<субсчет>]
            // extRef                             -  Внешняя ссылка (комментарий)
            DateTime endTime = DateTime.MinValue; // Время окончания действия заявки

            //long orderNo;                         // Выходной параметр orderNo содержит номер зарегистрированной заявки
            string resultMsg;                     // Выходной параметр resultMsg содержит текстовое сообщение о результате операции

            //Находим ордер------------------------

            DataRow orderX = null;

            long cache = 0;

            if (orderSHcache.Length > 0)
                cache = long.Parse(orderSHcache, CultureInfo.InvariantCulture);


            foreach (DataRow order in Orders)
            {
                if ((long)order["OrderNo"] == cache)
                    orderX = order;
            }

            //-------------------------------------- 

            if (orderX == null)
            {
                if (orderSHcache.Length == 0 && price > 0)
                {
                    long orderNo;
                    int result = 0;
                    result = slot.AddOrder(account, BuySell, mktLimit, splitFlag, immCancel, entryType, marketMaker, SecBoard, SecCode, issueCode, price, vol, brokerRef, comment, endTime, out orderNo, out resultMsg);
                    if (result == 0)
                    {
                        NamePos.SaveOrderDB(orderNo.ToString(), string.Empty);
                    }
                    if (_del != null)
                        _del("Cобытие", resultMsg);
                }
            }

            else
            {
                // если заявка активна
                if ((string)orderX["Status"] == "A" || (string)orderX["Status"] == "O")
                {
                    if ((double)orderX["Price"] != price && price != 0)
                    {
                        long orderNo1;
                        long orderNo2;
                        int result = slot.MoveOrders(0, (long)orderX["OrderNo"], price, 0, comment, 0, 0, 0, comment, out orderNo1, out orderNo2, out resultMsg);
                        if (result == 0)
                            NamePos.SaveOrderDB(orderNo1.ToString(), string.Empty);
                        else
                            NamePos.SaveOrderDB(((long)orderX["OrderNo"]).ToString(), string.Empty);

                        if (_del != null)
                            _del("Cобытие", resultMsg);
                    }
                    // Если выставленна заявка с Ценой == 0, но при этом ордер активен и не исполнен
                    else if (price == 0)
                    {
                        int result;
                        result = slot.DeleteOrder((long)orderX["OrderNo"], out resultMsg);
                        if (result == 0)
                            NamePos.SaveOrderDB(string.Empty, string.Empty);

                        if (_del != null)
                            _del("Cобытие", resultMsg);
                    }
                }
                // если заявка отменена
                else if ((string)orderX["Status"] == "W")
                {
                    long orderNo;
                    int result = 0;
                    result = slot.AddOrder(account, BuySell, mktLimit, splitFlag, immCancel, entryType, marketMaker, SecBoard, SecCode, issueCode, price, vol, brokerRef, comment, endTime, out orderNo, out resultMsg);
                    NamePos.SaveOrderDB(orderNo.ToString(), string.Empty);

                    if (_del != null)
                        _del("Cобытие", resultMsg);

                }
                //----------------------------------------------------------------------

                // если заявка исполнена
                else if ((string)orderX["Status"] == "M")
                {
                    Position posSH = new Position()
                    {
                        SecirityPos = ((string)orderX["SecCode"]).Trim(),
                        Order_Open_No = (long)orderX["OrderNo"],
                        IsLong = false,
                        Quantiry = (int)orderX["Quantity"],
                        OpenPrice = (double)orderX["Price"],
                        CurrentPrice = (double)orderX["Price"] - ProfitFuture, //!!!!!!!!
                        Stop = 0,
                        ProfitFuture = ProfitFuture,
                        Comment = "SH$" + DateTime.Now.Ticks.ToString(),
                        IsActive = true,
                        OpenDate = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)
                    };


                    NamePos.SaveOrderDB(string.Empty, string.Empty);
                    posSH.SavePosDB();

                    if (_del != null)
                        _del("Cобытие", "Открыта SH - pos : sec: " + posSH.SecirityPos + " | номер " + posSH.Comment + " | объем " + posSH.Quantiry + " | цена " + posSH.OpenPrice);
                }
            }
        }

        // Возвращает стакан котировак
        public static void StakanUp(string SecCode, DataTable OrderBook, out DataOrderBook[] bq, out DataOrderBook[] sq)
        {
            bq = new DataOrderBook[] { new DataOrderBook() { Price = 0, Quantity = 0 } };
            sq = new DataOrderBook[] { new DataOrderBook() { Price = 0, Quantity = 0 } };

            DataRow[] Array = OrderBook != null ? OrderBook.Select() : null;

            IList<DataOrderBook> BQ = new List<DataOrderBook>();
            IList<DataOrderBook> SQ = new List<DataOrderBook>();
            if (OrderBook != null)
            {
                foreach (var item in Array)
                {
                    if (item["BuySell"].ToString() == "B")
                    {
                        BQ.Add(new DataOrderBook { Price = (double)item["Price"], Quantity = (int)item["Quantity"] });
                    }

                    if (item["BuySell"].ToString() == "S")
                    {
                        SQ.Add(new DataOrderBook { Price = (double)item["Price"], Quantity = (int)item["Quantity"] });
                    }
                }

                bq = (BQ.Reverse()).ToArray();
                sq = SQ.ToArray();

            }
        }

        //--------------------------------------------------

        public static async void SaveOrderDB(this string name, string OpenOrder, string CloseOrder)
        {
            using (var db = new ApplicationContext())
            {
                var ord = db.Orders.SingleOrDefault(p => p.OrderName == name);
                ord.OpenOrderNo = OpenOrder;
                ord.CloseOrderNo = CloseOrder;
                await db.SaveChangesAsync();
            }
        }

        public static async void SavePosDB(this Position pos)
        {
            using (var db = new ApplicationContext())
            {
                db.Positions.Add(pos);
                await db.SaveChangesAsync();
            }
        }

        public static Position GetPosition(string PosName)
        {
            using (var db = new ApplicationContext())
            {
                Position pos = null;
                if(db.Positions.Count() > 0)
                    pos = db.Positions.SingleOrDefault(p => p.Comment.Contains(PosName + "$"));
                return pos;
            }
        }

        public async static void DelPosition(this string PosName)
        {
            using (var db = new ApplicationContext())
            {
                var pos = db.Positions.SingleOrDefault(p => p.Comment.Contains(PosName + "$"));
                db.Positions.Remove(pos);
                await db.SaveChangesAsync();
            }
        }

        public static Order GetOrder(string OrdName)
        {
            using (var db = new ApplicationContext())
            {
                var ord = db.Orders.SingleOrDefault(p => p.OrderName == OrdName);
                return ord;
            }
        }

        public async static void WrToLogDB(string source, string str)
        {
            using (var db = new ApplicationContext())
            {
                db.LogDatas.Add(new LogData() { LogToString = $"{source} | {str} | {DateTime.Now.ToString()}" });
                await db.SaveChangesAsync();
            }
        }
    }
}
