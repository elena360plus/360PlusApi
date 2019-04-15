
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace _360PlusPlugin.Models
{
    public class OrderSpire : baseSpire
    {

        //public bool backordered
        //{
        //    get; set;
        //}

        //public string invoiceDate
        //{
        //    get; set;
        //}
        //public int subtotalOrdered
        //{
        //    get; set;
        //}
        //public int batchNo
        //{
        //    get; set;
        //}
        public string termsCode
        {
            get; set;
        }
        //public decimal grossProfit2
        //{
        //    get; set;
        //}
        //public string currency
        //{
        //    get; set;
        //}
        //public string termsText
        //{
        //    get; set;
        //}
        //public int freight
        //{
        //    get; set;
        //}
        //public string profitCentre
        //{
        //    get; set;
        //}
        //public decimal grossProfitMargin
        //{
        //    get; set;
        //}
        //public string orderNo
        //{
        //    get; set;
        //}
        //public decimal total
        //{
        //    get; set;
        //}
        //public int id
        //{
        //    get; set;
        //}
        //public decimal totalCostAverage
        //{
        //    get; set;
        //}
        //public string salespersonNo
        //{
        //    get; set;
        //}
        //public decimal baseTotal
        //{
        //    get; set;
        //}
        //public string customerPO
        //{
        //    get; set;
        //}
        //public int subtotal
        //{
        //    get; set;
        //}
        //public string jobAccountNo
        //{
        //    get; set;
        //}
        //public string created
        //{
        //    get; set;
        //}
        //public decimal grossProfit
        //{
        //    get; set;
        //}
        //public string percentPaid
        //{
        //    get; set;
        //}
        //public string modifiedBy
        //{
        //    get; set;
        //}

        public string shipCode
        {
            get; set;
        }
        public string fob
        {
            get; set;
        }
        //public decimal totalCostCurrent2
        //{
        //    get; set;
        //}
        //public string phaseId
        //{
        //    get; set;
        //}
        /// <summary>
        /// O - sales order?
        /// </summary>
        //public string type
        //{
        //    get; set;
        //}
        //public decimal totalCostAverage2
        //{
        //    get; set;
        //}
        ///// <summary>
        /// O - sales order?
        /// </summary>
    //public string status
    //    {
    //        get; set;
    //    }    
        //public string division
        //{
        //    get; set;
        //}
        //public string location
        //{
        //    get; set;
        //}
        //public string trackingNo
        //{
        //    get; set;
        //}
        //public string shippingCarrier
        //{
        //    get; set;
        //}
        /// <summary>
        /// nullable
        /// </summary>
        //public string amountUnpaid
        //{
        //    get; set;
        //}
        //public decimal totalCostCurrent
        //{
        //    get; set;
        //}
        //public string referenceNo
        //{
        //    get; set;
        //}
        public string orderDate
        {
            get; set;
        }
        /// <summary>
        /// int or decimal?
        /// </summary>
        //public int discount
        //{
        //    get; set;
        //}
        ///// <summary>
        ///// int or decimal?
        ///// </summary>
        //public int totalDiscount
        //{
        //    get; set;
        //}
        //public string territoryCode
        //{
        //    get; set;
        //}
        //public string createdBy
        //{
        //    get; set;
        //}
        //public decimal grossProfitAvg
        //{
        //    get; set;
        //}
        //public bool hold
        //{
        //    get; set;
        //}
        //public int? amountPaid
        //{
        //    get; set;
        //}
        //public string jobNo
        //{
        //    get; set;
        //}
        //public string addrID2
        //{
        //    get; set;
        //}
        //public string modifiedDateTime
        //{
        //    get; set;
        //}
        //public string invoiceNo
        //{
        //    get; set;
        //}
        ////public string createdDateTime
        ////{
        ////    get; set;
        ////}
        //public int subTotal2
        //{
        //    get; set;
        //}
        //public string shipDate
        //{
        //    get; set;
        //}
        ////public string modified
        ////{
        ////    get; set;
        ////}
        //public decimal total2
        //{
        //    get; set;
        //}
        //public decimal grossProfitAvgOrder
        //{
        //    get; set;
        //}
        //public decimal grossProfitPctAvg
        //{
        //    get; set;
        //}
        //public string jobAccount
        //{
        //    get; set;
        //}
        //public string ship_id
        //{
        //    get; set;
        //}
        //public decimal grossProfitPctCurr
        //{
        //    get; set;
        //}
        public string requiredDate
        {
            get; set;
        }


        public OrderItemSpire[] items
        {
            get; set;
        }

        public AddressSpire shippingAddress
        {
            get; set;
        }

        //public AddressSpire address
        //{
        //    get; set;
        //}
        //public LinksSpire links
        //{
        //    get; set;
        //}

        public CustomerSpire customer
        {
            get; set;
        }

        //public ContactSpire contact
        //{
        //    get; set;
        //}



    }
}
