using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace ApiTest
{
    public class ApiException : Exception
    {
        public ApiException() : base() { }
        public ApiException(string message) : base(message) { }
        public ApiException(string message, Exception e) : base(message, e) { }

    }

  


    /// <summary>
    /// API client used to authenticate a user's access to a company, send requests, and receive responses
    /// </summary>
    public class ApiClient
    {
        // URL for API access; port may need to be changed if non-standard
       // const string BaseUrl = "https://localhost:10880/api/v1";
        readonly public string Company;
        RestClient _client;
        readonly string _baseUrl;
        readonly string _username;
        readonly string _password;

        /// <summary>
        /// Instantiate an ApiClient
        /// </summary>
        /// <param name="company">Company short name</param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public ApiClient(string company, string username, string password, string baseUrl)
        {
            Company = company;
            _username = username;
            _password = password;
            _baseUrl = baseUrl;
            _client = Client();
        }

        RestClient Client()
        {
            // Accept self-signed certificate
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;

            var client = new RestClient();
            client.BaseUrl = new Uri(_baseUrl);
            client.Authenticator = new HttpBasicAuthenticator(_username, _password);
            client.FollowRedirects = false;
            return client;
        }

     

        public string Execute<T>(RestRequest request) where T : new()
        {

            
           var response = _client.Execute<T>(request);
            var content = response.Content;
            
            HttpStatusCode code = response.StatusCode;
            if (code == HttpStatusCode.Unauthorized)
                throw new ApiException("Username or password incorrect");

  
                if (code != HttpStatusCode.BadRequest &&
                code != HttpStatusCode.InternalServerError && response.ErrorException != null)
                throw new ApplicationException(response.ErrorMessage,  response.ErrorException);

            //if (code == HttpStatusCode.OK || code == HttpStatusCode.NoContent)
            //    return response.Data;

            if (code == HttpStatusCode.Created)
            {
                var responseURI = response.ResponseUri;
                // Manually follow header redirect
                foreach (var p in response.Headers)
                {
                    if (p.Name == "Location")
                    {
                        var secondRequest = new RestRequest();
                        var location = (string)p.Value;
                        secondRequest.Resource = location.Remove(0, _client.BaseUrl.ToString().Length);
                        var id = location.Remove(0, responseURI.ToString().Length);

                        //  return response;
                        //   return Execute<T>(secondRequest);
                        return id;
                    }
                }
            }

         


            throw new ApiException(response.Content);
        }
    }


    public abstract class BaseObjectClient<T> where T : new()
    {
        protected ApiClient Client;

        public BaseObjectClient(ApiClient client)
        {
            Client = client;
        }

        string BaseResource
        {
            get
            {
                return string.Format("companies/{0}/", Client.Company);
            }
        }
                        
        abstract public string Resource { get; }

        /// <summary>
        /// List objects in resource collection
        /// </summary>
        /// <param name="start">Start offset in list</param>
        /// <param name="limit">Limit objects returned; defaults to 100</param>
        /// <param name="query">Search string</param>
        /// <param name="filter">Filter criteria</param>
        /// <returns></returns>
        //public List<T> List(int start=0, int limit=100, string query=null, object filter=null)
        //{
        //    var request = new RestRequest();

        //    if (start != 0)
        //        request.AddQueryParameter("start", start.ToString());

        //    if (limit != 100)
        //        request.AddQueryParameter("limit", limit.ToString());

        //    if (query != null)
        //        request.AddQueryParameter("q", query);

        //    if (filter != null)
        //    {
        //        string jsonFilter = JsonConvert.SerializeObject(filter);
        //        request.AddQueryParameter("filter", jsonFilter);
        //    }

        //    request.RequestFormat = DataFormat.Json;
        //    request.Resource = BaseResource + Resource;
        //    request.RootElement = "records";
        //    return Client.Execute<List<T>>(request);
        //}

        /// <summary>
        /// Get object by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public T Fetch(int id)
        //{
        //    var request = new RestRequest();
        //    request.RequestFormat = DataFormat.Json;
        //    request.Resource = BaseResource + Resource + id;
        //    return Client.Execute<T>(request);
        //}

        /// <summary>
        /// Create new object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Create(T obj)
        {

            string idNew = String.Empty;

            //var customer = new Customer();
            //  customer.custNo = "SAM1002";
            //  customer.name = "Test Accnt";
            //  inventory.id = 25121;
            //inventory.whse = "VA";
            //    inventory.partNo = "TEST1";
            //inventory.type = "N";
            //inventory.status = 0;
            //inventory.description = "Test Inventory";
            //inventory.backorderQty = "0";
            //inventory.committedQty = "0";
            //inventory.lotNumbered = false;
            //inventory.serialized = false;




            var request = new RestRequest(Method.POST);
            request.Parameters.Clear();
            request.AddHeader("Accept", "application/json");
            request.RequestFormat = DataFormat.Json;
            request.Resource = BaseResource + Resource;
            request.Parameters.Clear();
            request.AddBody(obj);

            return Client.Execute<T>(request);

        }

        /// <summary>
        /// Update existing object
        /// </summary>
        /// <param name="id"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Update(int id, T obj)
        {
            var request = new RestRequest(Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.Resource = BaseResource + Resource + id;
            request.AddBody(obj);
            return Client.Execute<T>(request);
        }

        /// <summary>
        /// Delete object by ID
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            var request = new RestRequest(Method.DELETE);
            request.RequestFormat = DataFormat.Json;
            request.Resource = BaseResource + Resource + id;
            Client.Execute<ErrorResponse>(request);
        }
    }


    public class ErrorResponse
    {
        public string message { get; set; }
        public string error_type { get; set; }
        public string traceback { get; set; }
    }
}
