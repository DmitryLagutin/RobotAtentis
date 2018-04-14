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


namespace AtentisPrimer
{
    public class Position                 //класс позиции
    {
        #region Поля Position

        public int Id { get; set; }
        public long Order_Open_No { get; set; }        //id одера открытия
        public long Order_Close_No { get; set; }       //id одера закрытия
        public bool IsLong { get; set; }               //направление позиции
        public double OpenPrice { get; set; }          //цена открытия позиции 
        public double Quantiry { get; set; }           //объем открытой позиции
        public string Comment { get; set; }            //комментарий
        public bool IsActive { get; set; }             //закрыта ли позиция
        public double ClosePrice { get; set; }         //цена закрытия
        public double Profit { get; set; }             //Профит по позиции
        public double CurrentPrice { get; set; }
        public double ProfitFuture { get; set; }
        public double Stop { get; set; }
        public string SecirityPos { get; set; }
        public DateTime OpenDate { get; set; }         //дата и время открытия
        public DateTime CloseDate { get; set; }        //дата и время открытия

        #endregion

        //закрытие позицию
        public async void CloseLE(Slot slot, IEnumerable<DataRow> Orders, string account, string brokerRef, string SecBoard, string SecCode, double price, int vol, string comment, string orderLXcache, string NamePos, double priceMetka)
        {

            // расчет стопа-----------------------------
            if (priceMetka - OpenPrice >= 0)
                Stop = Math.Abs(priceMetka - OpenPrice);
            else
                Stop = -Math.Abs(priceMetka - OpenPrice);

            using (var db = new ApplicationContext())
            {
                var _pos = db.Positions.Where(p => p.Comment == this.Comment).FirstOrDefault();
                _pos.Stop = this.Stop;
                await db.SaveChangesAsync();
            }

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

            if (orderLXcache.Length > 0)
                cache = long.Parse(orderLXcache, CultureInfo.InvariantCulture);

            foreach (DataRow order in Orders)
            {
                if ((long)order["OrderNo"] == cache)
                    orderX = order;
            }



            //-------------------------------------- 

            if (orderX == null)
            {
                if (orderLXcache.Length == 0)
                {
                    long orderNo;
                    int result = 0;
                    result = slot.AddOrder(account, BuySell, mktLimit, splitFlag, immCancel, entryType, marketMaker, SecBoard, SecCode, issueCode, price, vol, brokerRef, comment, endTime, out orderNo, out resultMsg);
                    if (result == 0)
                    {
                        NamePos.SaveOrderDB(string.Empty, orderNo.ToString());
                    }
                    
                    if(TradeHelper._del != null)
                        TradeHelper._del("Cобытие", resultMsg);
                }
            }

            else
            {
                // если заявка активна
                if ((string)orderX["Status"] == "A" || (string)orderX["Status"] == "O")
                {
                    if ((double)orderX["Price"] != price)
                    {
                        long orderNo1;
                        long orderNo2;
                        int result = slot.MoveOrders(0, (long)orderX["OrderNo"], price, 0, comment, 0, 0, 0, comment, out orderNo1, out orderNo2, out resultMsg);
                        if (result == 0)
                            NamePos.SaveOrderDB(string.Empty, orderNo1.ToString());
                        else
                            NamePos.SaveOrderDB(string.Empty, ((long)orderX["OrderNo"]).ToString());

                        if (TradeHelper._del != null)
                            TradeHelper._del("Cобытие", resultMsg);
                    }
                }
                // если заявка отменена
                else if ((string)orderX["Status"] == "W")
                {
                    long orderNo;
                    int result = 0;
                    result = slot.AddOrder(account, BuySell, mktLimit, splitFlag, immCancel, entryType, marketMaker, SecBoard, SecCode, issueCode, price, vol, brokerRef, comment, endTime, out orderNo, out resultMsg);
                    NamePos.SaveOrderDB(string.Empty, orderNo.ToString());
                    if (TradeHelper._del != null)
                        TradeHelper._del("Cобытие", resultMsg);
                }
                //----------------------------------------------------------------------


                // если заявка исполнена
                else if ((string)orderX["Status"] == "M")
                {
                    ClosePrice = (double)orderX["Price"];
                    Order_Close_No = (long)orderX["OrderNo"];
                    IsActive = false;
                    Profit = ClosePrice - OpenPrice;
                    CloseDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                    //Запись в лог-------------------------------------------
                    if (TradeHelper._del != null)
                        TradeHelper._del("Cобытие", "Закрыта LE - pos : sec: " + SecirityPos + " | номер " + Comment + " | объем " + Quantiry + " | цена " + ClosePrice);

                    NamePos.SaveOrderDB(string.Empty, string.Empty);
                    NamePos.DelPosition();
                }
            }
        }

        //закрытие позицию
        public async void CloseSH(Slot slot, IEnumerable<DataRow> Orders, string account, string brokerRef, string SecBoard, string SecCode, double price, int vol, string comment, string orderSXcache, string NamePos, double priceMetka) //Orders = Orders.Select()
        {

            //расчет стопа-------------------------
            if (OpenPrice - priceMetka >= 0)
                Stop = Math.Abs(OpenPrice - priceMetka);
            else
                Stop = -Math.Abs(OpenPrice - priceMetka);

            using (var db = new ApplicationContext())
            {
                var _pos = db.Positions.Where(p => p.Comment == this.Comment).FirstOrDefault();
                _pos.Stop = this.Stop;
                await db.SaveChangesAsync();
            }

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

            //long orderNo;                         // Выходной параметр orderNo содержит номер зарегистрированной заявки
            string resultMsg;                     // Выходной параметр resultMsg содержит текстовое сообщение о результате операции

            //Находим ордер------------------------

            DataRow orderX = null;

            long cache = 0;

            if (orderSXcache.Length > 0)
                cache = long.Parse(orderSXcache, CultureInfo.InvariantCulture);


            foreach (DataRow order in Orders)
            {
                if ((long)order["OrderNo"] == cache)
                    orderX = order;
            }

            //-------------------------------------- 

            if (orderX == null)
            {
                if (orderSXcache.Length == 0)
                {
                    long orderNo;
                    int result = 0;
                    result = slot.AddOrder(account, BuySell, mktLimit, splitFlag, immCancel, entryType, marketMaker, SecBoard, SecCode, issueCode, price, vol, brokerRef, comment, endTime, out orderNo, out resultMsg);
                    if (result == 0)
                    {
                        NamePos.SaveOrderDB(string.Empty, orderNo.ToString());
                    }
                    if (TradeHelper._del != null)
                        TradeHelper._del("Cобытие", resultMsg);
                }
            }

            else
            {
                // если заявка активна
                if ((string)orderX["Status"] == "A" || (string)orderX["Status"] == "O")
                {
                    if ((double)orderX["Price"] != price)
                    {
                        long orderNo1;
                        long orderNo2;
                        int result = slot.MoveOrders(0, (long)orderX["OrderNo"], price, 0, comment, 0, 0, 0, comment, out orderNo1, out orderNo2, out resultMsg);
                        if (result == 0)
                            NamePos.SaveOrderDB(string.Empty, orderNo1.ToString());
                        else
                            NamePos.SaveOrderDB(string.Empty, ((long)orderX["OrderNo"]).ToString());

                        if (TradeHelper._del != null)
                            TradeHelper._del("Cобытие", resultMsg);
                    }
                }
                // если заявка отменена
                else if ((string)orderX["Status"] == "W")
                {
                    long orderNo;
                    int result = 0;
                    result = slot.AddOrder(account, BuySell, mktLimit, splitFlag, immCancel, entryType, marketMaker, SecBoard, SecCode, issueCode, price, vol, brokerRef, comment, endTime, out orderNo, out resultMsg);
                    NamePos.SaveOrderDB(string.Empty, orderNo.ToString());

                    if (TradeHelper._del != null)
                        TradeHelper._del("Cобытие", resultMsg);
                }
                //----------------------------------------------------------------------


                // если заявка исполнена
                else if ((string)orderX["Status"] == "M")
                {

                    ClosePrice = (double)orderX["Price"];
                    Order_Close_No = (long)orderX["OrderNo"];
                    IsActive = false;
                    Profit = OpenPrice - ClosePrice;
                    CloseDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    //Запись в лог-------------------------------------------
                    if (TradeHelper._del != null)
                        TradeHelper._del("Cобытие", "Закрыта SH - pos : sec: " + SecirityPos + " | номер " + Comment + " | объем " + Quantiry + " | цена " + ClosePrice);
                    //Запись в файлы
                    NamePos.SaveOrderDB(string.Empty, string.Empty);
                    NamePos.DelPosition();
                }
            }
        }

    }
}
