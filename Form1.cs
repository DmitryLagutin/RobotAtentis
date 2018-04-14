using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Atentis.Connection;
using System.Threading;
using System.Globalization;
using System.Linq;
using static AtentisPrimer.InfoDataIn;
using System.Threading.Tasks;

namespace AtentisPrimer
{
    public delegate void PositionStateHandler(string message, string message1);

    public partial class Form1 : Form
    {
   
        public Form1()
        {
            InitializeComponent();
            InitEvents();
            InitSlot(); // Инициализация data_slot и trans_slot
            TradeHelper._del += AddEvent;
            TradeHelper._del += TradeHelper.WrToLogDB;
            
        }

        #region Основные методы

        public async void KillPosFile()
        {
            using (var db = new ApplicationContext())
            {
                //db.Positions.Add(new Position() { CloseDate = DateTime.Now, ClosePrice = 2323, Comment = "2323", CurrentPrice = 23232, IsActive = true, IsLong = true, OpenDate = DateTime.Now, OpenPrice = 2323, Order_Close_No = 23232, Order_Open_No = 23232, Profit = 23232, ProfitFuture = 23232, Quantiry = 1, SecirityPos = "2323", Stop = 2323 });
                //db.SaveChanges();
                foreach (var position in db.Positions)
                {
                    if(db.Positions.Count() > 0)
                    {
                        db.Positions.Remove(position);
                        await db.SaveChangesAsync();
                    }
                }

                foreach (var order in db.Orders)
                {
                    order.OpenOrderNo = string.Empty;
                    order.CloseOrderNo = string.Empty;
                    await db.SaveChangesAsync();
                }
            }

            AddEvent("Событие", "Удалили все позиции и ордера");
        }

        // Инициализация события для отрисовки
        public void InitEvents()
        {
            listView1.Clear();
            listView1.Columns.Add("N", 35, HorizontalAlignment.Right);
            listView1.Columns.Add("Date", 70, HorizontalAlignment.Left);
            listView1.Columns.Add("Time", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Source", 60, HorizontalAlignment.Left);
            listView1.Columns.Add("Event", 590, HorizontalAlignment.Left);
        }

        // Добавление события для отрисовки
        public void AddEvent(string source, string text)
        {
            string str = "";
            DateTime time = DateTime.Now;

            ListViewItem item = new ListViewItem((listView1.Items.Count + 1).ToString());
            for (int k = 1; k < listView1.Columns.Count; k++)
            {
                item.SubItems.Add("");
                string s = "";
                ColumnHeader col = listView1.Columns[k];
                if (col.Text == "Date")
                    s = time.ToShortDateString();
                else if (col.Text == "Time")
                    s = String.Format("{0}.{1:000}", time.ToLongTimeString(), time.Millisecond);
                else if (col.Text == "Source")
                    s = source;
                else if (col.Text == "Event")
                    s = text;
                item.SubItems[k].Text = s;
                str = str + s;
            }
            listView1.Items.Add(item);

            if (listView1.SelectedItems.Count == 1 && listView1.SelectedItems[0].Index == listView1.Items.Count - 2)
            {
                listView1.SelectedItems[0].Selected = false;
                item.Selected = true;
                item.Focused = true;
                item.EnsureVisible();
            }
            else if (listView1.SelectedItems.Count == 0)
                item.EnsureVisible();
        }

        // Соединение
        public void Connect()
        {
            Cursor.Current = Cursors.WaitCursor;
            data_slot.rqs = new RequestSocket(data_slot);
            data_slot.rqs.evhLogLine += new TableEventHandler(RQS_LogLine);
            data_slot.rqs.evhSynchronized += new TableEventHandler(RQS_Synchronized);
            data_slot.rqs.evhNewSession += new TableEventHandler(RQS_NewSession);
            data_slot.rqs.evhUpdateAllTableViews += new TableListEventHandler(RQS_UpdateAllTableViews);
            data_slot.rqs.Init();
            //--------То что касается таймера
            timer1.Interval = 179000;
            timer1.Tick += new EventHandler(timer1_Tick);
            //--------
            data_slot.Start();

            Cursor.Current = Cursors.Default;
        }

        // Рассоединение
        private void Disconnect()
        {
            Cursor.Current = Cursors.WaitCursor;
            if (data_slot.rqs != null)
            {
                data_slot.rqs.evhLogLine -= new TableEventHandler(RQS_LogLine);
                data_slot.rqs.evhSynchronized -= new TableEventHandler(RQS_Synchronized);
                data_slot.rqs.evhNewSession -= new TableEventHandler(RQS_NewSession);
                data_slot.rqs.evhUpdateAllTableViews -= new TableListEventHandler(RQS_UpdateAllTableViews);
                data_slot.Disconnect();
            }
            if (trans_slot != null)
            {
                string msg;
                timer1.Tick -= new EventHandler(timer1_Tick);
                trans_slot.CloseConnection(out msg);
                AddEvent(trans_slot.SlotID, msg);
                timer1.Stop();
            }
            Cursor.Current = Cursors.Default;
          
        }

        // Остановить робота
        private void StopRobot()
        {
            AddEvent("Robot", "---Robot Stop---");
            tsbStartRobot.Enabled = true;
            tsbStopRobot.Enabled = false;
            // перезапускаем таймер
            timer1.Stop();
            timer1.Start();
            RobotWork = false;
        }

        // Инициализировать слот
        private void InitSlot()
        {
            // Создаем data_slot
            data_slot = new Slot("DataSlot", Server, Port, Login, Password, false, "");
            data_slot.RefreshPeriod = 100; // Меньше 100 мc задавать нет смысла. это минимально возможное значение
            data_slot.evhSlotStateChanged += new SlotEventHandler(SLOT_SlotState);

            // Создаем trans_slot
            trans_slot = new Slot("TransSlot", Server, 7800, Login, Password, false, "");
            trans_slot.evhSlotStateChanged += new SlotEventHandler(SLOT_SlotState);

            table_Sec = data_slot.AddTable(new Table("SECURITIES", "SECURITIES", "", "", "", ""));
            table_Sec.evhAddRow += new TableDataEventHandler(Sec_evhAddRow);
            table_Sec.evhUpdateRow += new TableDataEventHandler(Sec_evhUpdateRow);

            table_Orders = data_slot.AddTable(new Table("ORDERS", "ORDERS", "", "", "", ""));
            table_Orders.Baseless = false; // ORDERS безбазовая таблица
            table_Orders.evhAddRow += new TableDataEventHandler(Orders_evhAddRow);
            table_Orders.evhUpdateRow += new TableDataEventHandler(Orders_evhUpdateRow);


            Si_OrderBook = data_slot.AddOrderbook(SecBoard_Si, SecCode_Si);
            Si_OrderBook.evhAddRow += new TableDataEventHandler(OrderBook_evhAddRow);
            Si_OrderBook.evhUpdateRow += new TableDataEventHandler(OrderBook_evhUpdateRow);

            Ri_OrderBook = data_slot.AddOrderbook(SecBoard_Ri, SecCode_Ri);
            Ri_OrderBook.evhAddRow += new TableDataEventHandler(OrderBook_evhAddRow);
            Ri_OrderBook.evhUpdateRow += new TableDataEventHandler(OrderBook_evhUpdateRow);
        }

        #endregion

        #region Кнопки и формы

        // Состояние формы если она закрывется 
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopRobot();
            Disconnect();
            Application.DoEvents();
        }

        // Кнопка очистки папок файлов с позициями и ордерами
        private void button_ClearFile_Click(object sender, EventArgs e)
        {
            if(RobotWork == false)
            {
                KillPosFile();
            }
            else
            {
                AddEvent("Robot", "Торговый робот работает!!! Hельзя удалить файлы во время работы робота");
            }
        }

        // Кнопка подключения к слоту
        private void tsbConnect_Click(object sender, EventArgs e)
        {
            if (data_slot.State == SlotState.Disconnected)
            {
                Connect();
            }
        }

        // Кнопка отключения от слота
        private void tsbDisconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        // Кнопка запуска робота
        private void tsbStartRobot_Click(object sender, EventArgs e)
        {
            if (data_slot.State == SlotState.Connected)
            {
                tsbStartRobot.Enabled = false;
                tsbStopRobot.Enabled = true;
                AddEvent("Robot", "---Robot Start---");
                // Включаем метод для торговли
                RobotWork = true;

            }

            else
            {
                AddEvent("Robot", "Вы не подключились к слоту");
            }
        }

        // Кнопка остановки работы робота
        private void tsbStopRobot_Click(object sender, EventArgs e)
        {
            StopRobot();
        }

        //  Указатель включения или отключения авто переподключения
        private void cbAutoNewSession_CheckedChanged(object sender, EventArgs e)
        {

        }

        // Панелька вывода информации
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // Вывод состояния скрипта в панеле внизу
        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        #endregion

        #region Обработчики событий

        public void RQS_LogLine(object sender, TableEventArgs e)
        {
            Slot slot = null;
            if (e.RequestSocket != null)
                slot = e.RequestSocket.slot;
            AddEvent(slot.SlotID, e.Message);
        }

        public void RQS_Synchronized(object sender, TableEventArgs e)
        {
            Slot slot = null;
            if (e.RequestSocket != null)
                slot = e.RequestSocket.slot;
            AddEvent(slot.SlotID, "Synchronized");
            // Заказываем OrderBook, обращаться к данным будем в обрабочике событий RQS_UpdateAllTableViews через  DataView OrderBook
            OrderBook_Si = data_slot.GetOrderbook(SecBoard_Si, SecCode_Si);
            OrderBook_Ri = data_slot.GetOrderbook(SecBoard_Ri, SecCode_Ri);
            Orders = data_slot.GetTable("ORDERS");

            // получаем BROKERREF и ACCOUNT, они будут нужны постановки заявки
            BROKERREF = slot.BrokerRef;
            ACCOUNT = slot.TrdAccID;
            
            //Определяем тип сервера АЛОР-Трейд (если ФОРТС то будем использовать MoveOrder, если ММВБ то AddOrder и DeleteOrder) 
            SERVERTYPE = slot.ServerType;
            
            
            // По событию Synchronized коннектим trans_slot
            Thread.Sleep(1000); // Ждем 1000 мс так как второй коннект сервер не примет раньше чем через 1000 мс
            string msg;
            trans_slot.OpenConnection(out msg);
            AddEvent(trans_slot.SlotID, msg);
            timer1.Start();
         }

        public void SLOT_SlotState(object sender, SlotEventArgs e)
        {
            if (e.Slot.SlotID == "DataSlot")
            {
                if (e.State == SlotState.Disconnected)
                {
                    tsbDisconnect.Enabled = false;
                    tsbConnect.Enabled = true;
                }
                if (e.State == SlotState.Connected)
                {
                    tsbDisconnect.Enabled = true;
                    tsbConnect.Enabled = false;
                }
                string str = "DataSlot :" + e.State;
                statusStrip1.Items["StatusData"].Text = str;
            }
            if (e.Slot.SlotID == "TransSlot")
            {
                string str = "TransSlot :" + e.State;
                statusStrip1.Items["StatusTrans"].Text = str;
            }
        }
      
        public void RQS_NewSession(object sender, TableEventArgs e)
        {
            // событие NewSession приходит в случае изменения номера сессии на сервере АЛОР-Трейд
            // номер сесии меняется в случает планового(новый торговый день) или внепланового рестарта сервера АЛОР-Трейд
            listView1.Items.Clear();
            if (trans_slot != null)
            {
                string msg;
                timer1.Tick -= new EventHandler(timer1_Tick);
                trans_slot.CloseConnection(out msg);
                AddEvent(trans_slot.SlotID, msg);
                timer1.Stop();
            }
                        
            data_slot.rqs.evhLogLine -= new TableEventHandler(RQS_LogLine);
            data_slot.rqs.evhSynchronized -= new TableEventHandler(RQS_Synchronized);
            data_slot.rqs.evhNewSession -= new TableEventHandler(RQS_NewSession);
            data_slot.rqs.evhUpdateAllTableViews -= new TableListEventHandler(RQS_UpdateAllTableViews);
     
            // Если флаг AutoNewSession установлен, то коннектимся к серверу
            if (cbAutoNewSession.Checked)
            {
               AddEvent("Primer", "Start AutoNewSession");
               Connect();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // этот таймер срабатывает раз в 180 сек, при условии, что не было за это время ни одной транзакциии
            // это необходимо, так как slot подключенный к серверу с помощью метода OpenConnection сам не отслеживает состояние TCP соединения, а
            // некоторые сетевые устройтва (NAT, Firewall или Proxy) могут при неактивности TCP соединения закрывать его по таймауту.
            trans_slot.RefreshAllTables();
        }

        public void RQS_UpdateAllTableViews(object sender, TableListEventArgs e)
        {
            foreach (Table t in e.List)
            {
                if (t.Name == "ORDERS")
                {
                    DataRow[] Array = Orders != null ? Orders.Select() : null;
                }
                //------------------------------------------
                else if (t.Name == "ORDERBOOK" && t.Param2 == SecCode_Si)
                {
                    TradeHelper.StakanUp(SecCode_Si, OrderBook_Si, out bq_Si, out sq_Si);
                }
                 
                //-----------------------------------------
                else if (t.Name == "ORDERBOOK" && t.Param2 == SecCode_Ri)
                {
                    TradeHelper.StakanUp(SecCode_Ri, OrderBook_Ri, out bq_Ri, out sq_Ri);

                }
            }
            //-----------------------------------------
            Trading.TradeFunc();

        }
     
        //------Обработчики связанные непосредственно с поступаэщими данными----

        public void Orders_evhAddRow(object sender, TableDataEventArgs e)
        {
            DataRow dr = e.DataRow;
        }

        public void Orders_evhUpdateRow(object sender, TableDataEventArgs e)
         {
             DataRow dr = e.DataRow;
         }

        public void Sec_evhAddRow(object sender, TableDataEventArgs e)
        {
            DataRow dr = e.DataRow;
        }

        public void Sec_evhUpdateRow(object sender, TableDataEventArgs e)
        {
            DataRow dr = e.DataRow;
        }

        public void OrderBook_evhAddRow(object sender, TableDataEventArgs e)
        {
            DataRow dr = e.DataRow;
        }

        public void OrderBook_evhUpdateRow(object sender, TableDataEventArgs e)
        {
            DataRow dr = e.DataRow;
        }

        #endregion
    }
    
}
