using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using xrm = SpirePlusPlugin;
using System.Net;

namespace _360PlusPlugin.Utility 
{
    public class SpireHelper 
    {

        #region Spire Methods

        public string SpirePostMethod_Account(xrm.XrmServiceContext ctx, Guid _AccountId)
        {
            string Error = String.Empty;
            string resultPostCustomer = String.Empty;

            var newAccount = (from ac in ctx.AccountSet where ac.AccountId == _AccountId select ac).FirstOrDefault();
            if (newAccount == null) return "Error: New Account is Empty";
            Models.CustomerSpire modelCustomerFromCRM = GetModelAccountFromCRM(ctx, newAccount);

            try
            {
                string url = string.Empty;

                


            }
            catch (Exception e)
            {

                return String.Empty;

                // return Request.CreateResponse(HttpStatusCode.NoContent, "Server error!");
            }


            return String.Empty;
        }

        public static Models.CustomerSpire  GetModelAccountFromCRM(xrm.XrmServiceContext ctx, xrm.Account ac)
        {
            #region Get Currency

            string CurrencyISOCode = String.Empty;
            if (ac.TransactionCurrencyId != null && ac.TransactionCurrencyId.Id != Guid.Empty)
            {
                 CurrencyISOCode = (from cc in ctx.TransactionCurrencySet where cc.TransactionCurrencyId == ac.TransactionCurrencyId.Id select cc.ISOCurrencyCode).FirstOrDefault();
             }

            #endregion

            #region Create Spire Customer Model
            return new Models.CustomerSpire()
            {

                name = ac.Name,
                code = ac.AccountNumber,
                customerNo = ac.AccountNumber,
                 hold = (bool)ac.CreditOnHold,
                 reference = ac.tsp_Reference,
                applyFinanceCharges = ac.tsp_ApplyFinanceCharges,
                creditType = ac.tsp_CreditType.Value,
                creditLimit = decimal.Parse(ac.CreditLimit.Value.ToString()),
                currency = CurrencyISOCode

            };
            #endregion
        }


        #endregion


    }
}
