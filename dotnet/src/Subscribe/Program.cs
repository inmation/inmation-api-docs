using inmation.api.model;
using inmation.api.model.rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace inmation.api.client.example.Subscribe
{
    class Program
    {
        private const string WebSocketUrl = "ws://localhost:8002/ws";
        private const string Username = "USERNAME";
        private const string Password = "PASSWORD";
        private static Client _client;

        static void Main(string[] args)
        {
            // Make sure the following Generic items exist in the system
            List<Identity> identityList = new List<Identity>();
            identityList.Add(new Identity("/System/Core/Examples/Demo Data/Process Data/DC4711"));
            identityList.Add(new Identity("/System/Core/Examples/Demo Data/Process Data/DC666"));

            // Create the API client
            _client = CreateApiClient(Username, Password);

            // Subscribe DataChanged
            SubscribeDataChanged(identityList).Wait();

            // For demo purpose we wait some time to be able to receive data changes before we unsubscribe again.
            Task.Delay(5000).Wait();

            // UnSubscribe DataChanged (by providing an empty list).
            SubscribeDataChanged(new List<Identity>()).Wait();

            Console.ReadLine();
            _client.Dispose();
        }

        /// <summary>
        /// Subscribes to DataChanged events for the provided items.
        /// </summary>
        /// <param name="items">List of Identity instances to subscribe to.</param>
        private static async Task SubscribeDataChanged(List<Identity> items)
        {
            if (items.Any())
            {
                LogResult();
            }
            else
            {
                Console.WriteLine("Unsubscribe all DataChanged subscriptions.");
            }

            SubscriptionResponse subscriptionResponse = await _client.SubscribeAsync(EnumSubscriptionType.DataChanged, items);

            if (subscriptionResponse.Error != null)
            {
                Console.WriteLine(string.Format("An error has occurred : {0}", subscriptionResponse.Error?.First().Message));
            }
            else
            {
                foreach (ItemValue itemValue in subscriptionResponse.Data)
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
        private static Client CreateApiClient(string username = null, string password = null)
        {
            Client apiClient = new Client();
            try
            {
                apiClient.OnDataChanged += new Client.OnDataChangedHandler(OnDataChanged);
                apiClient.OnConnectionChanged += OnConnectionStateChanged;
                apiClient.OnError += OnError;

                ConnectOptions options = new ConnectOptions(username, password);

                // Connect and authenticate. By providing credentials to the connectWs method, the credentials will be stored in the session.
                ConnectionResponse connectResponse = apiClient.ConnectWs(WebSocketUrl, options).Result;
                if (connectResponse.Error != null)
                {
                    Console.WriteLine(string.Format("Connect failed: {0}", connectResponse.Error?.First().Message));
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

        private static void OnDataChanged(List<ItemValue> items)
        {
            foreach (ItemValue itemValue in items)
            {
                Console.WriteLine("OnDataChanged: {0}", itemValue);
            }
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