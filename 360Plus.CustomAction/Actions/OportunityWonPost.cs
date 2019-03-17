using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using _360PlusPlugin;
using System.IO;
using System.Net;
using _360PlusPlugin.Utility;
using System.Text;

namespace _360Plus.CustomAction.Actions
{
    public class OportunityWonPost : PluginBase
    {
        #region Constructor/Configuration
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public OportunityWonPost(string unsecure, string secureConfig)
            : base(typeof(OportunityWonPost))
        {
            _secureConfig = secureConfig;
            _unsecureConfig = unsecure;
        }
        #endregion

        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));

            IPluginExecutionContext context = localContext.PluginExecutionContext;
            IOrganizationService service = localContext.OrganizationService;
            ITracingService tracingService = localContext.TracingService;


            //string urlInventory = "https://localhost:10880/api/v1/companies/inspire2_10/sales/orders/";
            string urlInventory = "https://localhost:10880/api/v1/companies/inspire2_10/inventory/items";

            String username = "SPIRE";
            String password = "12345";



            try
            {
                // Accept self-signed certificate
                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => true;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(urlInventory);


                //way I

                //String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                //httpWebRequest.Headers.Add("Authorization", "Basic " + encoded);
                //httpWebRequest.Method = "GET";
                //httpWebRequest.PreAuthenticate = true;


                //way II

                NetworkCredential networkCredential = new NetworkCredential(username, password);

                Uri u = new Uri(urlInventory);
                CredentialCache credentialCache = new CredentialCache();
                credentialCache.Add(u, "Basic", networkCredential);

                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Credentials = credentialCache;



                using (HttpWebResponse webResponse = httpWebRequest.GetResponse() as HttpWebResponse)
                {
                    if (webResponse.StatusCode == HttpStatusCode.OK)
                    {

                        Stream responseStream = webResponse.GetResponseStream();

                        StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

                        string jsonText = myStreamReader.ReadToEnd();

                    }
                }
            }
            catch (WebException ex)
            {
                ExceptionRouter.handlePluginException(ex, ChildClassName, tracingService);
            }
        }
    }
}