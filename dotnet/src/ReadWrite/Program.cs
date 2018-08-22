using inmation.api.model;
using inmation.api.model.rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace inmation.api.client.example.ReadWrite
{
    class Program
    {
        private const string WebSocketUrl = "ws://localhost:8002/ws";
        private const string Username = "USERNAME";
        private const string Password = "PASSWORD";
        private static Client _client;

        static void Main(string[] args)
        {
            // Make sure the following Variable / Holder items exist in the system
            List<Identity> identityList = new List<Identity>();
            identityList.Add(new Identity("/System/Core/Examples/Demo Data/Process Data/DC4711"));
            identityList.Add(new Identity("/System/Core/Examples/Demo Data/Process Data/DC666"));

            // Create the API client
            _client = CreateApiClient(Username, Password).Result;

            // Write Items
            List<ItemValue> writeItems = identityList.Select(n => new ItemValue() { Path = n.Path, Value = "test", Quality = 0, Timestamp = DateTime.UtcNow.AddDays(-1) }).ToList();
            WriteMultipleItemsAtOnce(writeItems).Wait();

            // Read items
            ReadMultipleItemsAtOnce(identityList).Wait();

            Console.ReadLine();
            _client.Dispose();
        }

        /// <summary>
        /// Reads multiple items in one single request.
        /// Currently only reading based on the path of an item or property is supported.
        /// </summary>
        /// <param name="items">List of Identity instances to read.</param>
        private static async Task ReadMultipleItemsAtOnce(List<Identity> items)
        {
            LogResult();

            ReadResponse readResponse = await _client.ReadAsync(items);

            if (readResponse.Error != null)
            {
                Console.WriteLine(string.Format("An error has occurred : {0}", readResponse.Error?.First().Message));
            }
            else
            {
                foreach (ItemValue itemValue in readResponse.Data)
                {
                    Console.WriteLine("ItemValue: {0}", itemValue);
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Writes multiple items in one single request.
        /// Currently only writing based on item path and VQT is supported.
        /// </summary>
        /// <param name="items">List of ItemValue instances to write.</param>
        private static async Task WriteMultipleItemsAtOnce(List<ItemValue> items)
        {
            LogResult();

            WriteResponse writeResponse = await _client.WriteAsync(items);

            if (writeResponse.Error != null)
            {
                Console.WriteLine(string.Format("An error has occurred : {0}", writeResponse.Error?.First().Message));
            }
            else
            {
                foreach (ItemValue itemValue in writeResponse.Data)
                {
                    Console.WriteLine("ItemValue: {0}", itemValue);
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Creates a connected client. 
        /// In case credentials are provided, the user will be authenticated and the credentials will be stored in the session.
        /// By providing credentials in the constructor of the apiClient it is not necessary to provide credentials for each individual request.
        /// during the session.
        /// </summary>
        private static async Task<Client> CreateApiClient(string username = null, string password = null)
        {
            Client apiClient = new Client();
            try
            {
                apiClient.OnConnectionChanged += OnConnectionStateChanged;
                apiClient.OnError += OnError;

                ConnectOptions options = new ConnectOptions(username, password);

                // Connect and authenticate. By providing credentials to the connectWs method, the credentials will be stored in the session.
                ConnectionResponse connectResponse = await apiClient.ConnectWs(WebSocketUrl, options);
                if (connectResponse.Error != null)
                {
                    Console.WriteLine("Connect failed: {0}", connectResponse.Error?.First().Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred during client creation: {0}", ex.Message);
            }
            Console.WriteLine("");
            return apiClient;
        }

        private static void LogResult([CallerMemberName] string callingMethodName = "")
        {
            Console.WriteLine("Result of {0}:", callingMethodName);
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