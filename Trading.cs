using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AtentisPrimer.TradeHelper;
using static AtentisPrimer.InfoDataIn;

namespace AtentisPrimer
{
    public static class Trading
    {
        public static void TradeFunc()
        {
            if (bq_Si.Count() > 1)
            {
                //Забираем позиции и ордера из базы данных

                Position posLE = GetPosition("LE");
                string orderLE = GetOrder("LE").OpenOrderNo;
                string orderLX = GetOrder("LE").CloseOrderNo;



                Position posSH = GetPosition("SH");
                string orderSH = GetOrder("SH").OpenOrderNo;
                string orderSX = GetOrder("SH").CloseOrderNo;

                #region ЦЕНЫ ОТКРЫТИЯ

                double OpenLE = RobotWork == true ? bq_Si[0].Price : 0;
                double ProfitFutureLE = 5;

                double OpenSH = RobotWork == true ? sq_Si[0].Price : 0;
                double ProfitFutureSH = 5;

                #endregion

                #region ЗАКРЫТИЕ

                if (posLE != null)
                {
                    if (posLE.Stop <= -20)
                        posLE.CurrentPrice = sq_Si[0].Price;
                    else if (RobotWork == false)
                        posLE.CurrentPrice = sq_Si[0].Price;
                }

                if (posSH != null)
                {
                    if (posSH.Stop <= -20)
                        posSH.CurrentPrice = bq_Si[0].Price;
                    else if (RobotWork == false)
                        posSH.CurrentPrice = bq_Si[0].Price;
                }


                #endregion


                //BUY
                if (posLE == null)
                    trans_slot.OpenLE(Orders.Select(), ACCOUNT, BROKERREF, SecBoard_Si, SecCode_Si, OpenLE, 1, "x", orderLE, "LE", ProfitFutureLE);
                else
                    posLE.CloseLE(trans_slot, Orders.Select(), ACCOUNT, BROKERREF, SecBoard_Si, SecCode_Si, posLE.CurrentPrice, 1, "x", orderLX, "LE", sq_Si[0].Price);

                //SELL
                if (posSH == null)
                    trans_slot.OpenSH(Orders.Select(), ACCOUNT, BROKERREF, SecBoard_Si, SecCode_Si, OpenSH, 1, "x", orderSH, "SH", ProfitFutureSH);
                else
                    posSH.CloseSH(trans_slot, Orders.Select(), ACCOUNT, BROKERREF, SecBoard_Si, SecCode_Si, posSH.CurrentPrice, 1, "x", orderSX, "SH", bq_Si[0].Price);

            }

        }
    }
}
