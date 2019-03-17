using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace _360PlusPlugin.Utility
{
    public class ExceptionRouter
    {

        public static void handlePluginException(Exception E, string ChildClassName, ITracingService TracingService)
        {

            try
            {
                FaultException<OrganizationServiceFault> fex = E as FaultException<OrganizationServiceFault>;
                if (!ReferenceEquals(fex, null))
                    throw fex;

                InvalidPluginExecutionException pex = E as InvalidPluginExecutionException;
                if (!ReferenceEquals(pex, null))
                    throw pex;

                throw E;

            }

            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException(string.Format("Service Fault: {0}, InnerExeption : {1}", new object[] { ex.Message, ex.InnerException }));
            }
            catch (InvalidPluginExecutionException ex)
            {
                TracingService.Trace("{0}: {1}", new object[] { ChildClassName, ex.Message });
                throw;
            }

            catch (Exception ex)
            {
                TracingService.Trace("{0}: {1}", new object[] { ChildClassName, ex.Message });
                throw;
            }
            finally
            {
                //close azure service connection
            }


        }


        public static void handleOtherExceptions(Exception E)
        {
            throw new InvalidPluginExecutionException(OperationStatus.Failed, E.Message);
        }

    }
}
