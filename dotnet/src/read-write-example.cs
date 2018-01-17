using inmation.api;
using inmation.api.model;
using inmation.api.model.rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace inmation.WebApi.ClientSample
{
    class Program
    {
        private const string WebSocketUrl = "ws://localhost:8000/ws";
        private const string Username = "USERNAME";
        private const string Password = "PASSWORD";
        private static Client _client;

        static void Main(string[] args)
        {
            // Make sure the following Variable / Holder items exist in the system
            List<Identity> identityList = new List<Identity>();
            identityList.Add(new Identity("/System/Core/Simulation/Item100"));
            identityList.Add(new Identity("/System/Core/Simulation/Item200"));

            // Create the API client
            _client = CreateApiClient(Username, Password);

            // Write Items
            List<ItemValue> writeItems = identityList.Select(n => new ItemValue() { Path = n.Path, V = "test", Q = 0, T = DateTime.UtcNow.AddDays(-1) }).ToList();
            WriteMultipleItemsAtOnce(writeItems);

            // Read items
            ReadMultipleItemsAtOnce(identityList);

            Console.ReadLine();
            _client.Dispose();
        }

        /// <summary>
        /// Reads multiple items in one single request.
        /// Currently only reading based on the path of an item or property is supported.
        /// </summary>
        /// <param name="items">List of Identity instances to read.</param>
        private static void ReadMultipleItemsAtOnce(List<Identity> items)
        {
            Console.WriteLine($"Result of {MethodBase.GetCurrentMethod().Name}:");
            ReadResponse response = _client.ReadAsync(items).Result;
            foreach (ItemValue item in response.Data)
            {
                Console.WriteLine($"ItemValue: {item}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Writes multiple items in one single request.
        /// Currently only writing based on item path and VQT is supported.
        /// </summary>
        /// <param name="items">List of ItemValue instances to write.</param>
        private static void WriteMultipleItemsAtOnce(List<ItemValue> items)
        {
            Console.WriteLine($"Result of {MethodBase.GetCurrentMethod().Name}:");
            WriteResponse response = _client.WriteAsync(items).Result;
            foreach (ItemValue item in response.Data)
            {
                Console.WriteLine($"ItemValue: {item}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Creates a connected client. 
        /// In case credentials are provided, the user will be authenticated and the credentials will be stored in the session.
        /// By providing credentials in the constructor of the apiClient it is not necessary to provide credentials for each individual request.
        /// during the session.
        /// </summary>
        private static Client CreateApiClient(string username = null, string password = null)
        {
            Client apiClient = new Client();
            try
            {
                apiClient.OnConnectionChanged += OnConnectionStateChanged;
                apiClient.OnError += OnError;

                RpcOptions options = new RpcOptions();
                options.Username = username;
                options.Password = password;

                // Connect and authenticate. By providing credentials to the connectWs method, the credentials will be stored in the session.
                ConnectionResponse connectionresponse = apiClient.ConnectWs(WebSocketUrl, options).Result;
                if (connectionresponse.Error != null)
                {
                    Console.WriteLine(string.Format("Connect failed: {0}", connectionresponse.Error?.First().Message));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during client creation: {ex.Message}");
            }
            return apiClient;
        }

        #region EventHandlers

        private static void OnOpen()
        {
            Console.WriteLine("WebSocket connection opened!");
        }

        private static void OnConnectionStateChanged(WsConnectionInfo connectionInfo)
        {
            Console.WriteLine(connectionInfo);
        }

        private static void OnError(ApiErrorResponse errorResponse, long? requestId)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Error error in errorResponse.ErrorList)
            {
                sb.AppendLine(error.ToString());
                Console.WriteLine(error);
            }
        }

        private static void OnClose(bool wasClean, ApiErrorResponse errorResponse)
        {
            if (!wasClean)
            {
                Console.WriteLine("Connection is forcibly closed.");
                OnError(errorResponse, null);
            }
            Console.WriteLine("Connection is closed.");
        }

        #endregion
    }
}