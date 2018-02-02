using inmation.api.model;
using inmation.api.model.rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace inmation.api.client.example.Subscribe
{
    class Program
    {
        private const string WebSocketUrl = "ws://localhost:8000/ws";
        private const string Username = "USERNAME";
        private const string Password = "PASSWORD";
        private static Client _client;

        static void Main(string[] args)
        {
            // Make sure the following Generic items exist in the system
            List<Identity> identityList = new List<Identity>();
            identityList.Add(new Identity("/System/Core/Simulation/Saw100"));
            identityList.Add(new Identity("/System/Core/Simulation/Saw200"));

            // Create the API client
            _client = CreateApiClient(Username, Password);

            // Subscribe DataChanged
            SubscribeDataChanged(identityList);

            // For demo purpose we wait some time to be able to receive data changes before we unsubscribe again.
            Task.Delay(5000).Wait();

            // UnSubscribe DataChanged (by providing an empty list).
            SubscribeDataChanged(new List<Identity>());

            Console.ReadLine();
            _client.Dispose();
        }

        /// <summary>
        /// Subscribes to DataChanged events for the provided items.
        /// </summary>
        /// <param name="items">List of Identity instances to subscribe to.</param>
        private static void SubscribeDataChanged(List<Identity> items)
        {
            if (items.Any())
            {
                Console.WriteLine("Result of {0}:", MethodBase.GetCurrentMethod().Name);
            }
            else
            {
                Console.WriteLine("Unsubscribe all DataChanged subscriptions.");
            }

            Task<SubscriptionResponse> subscribeTask = _client.SubscribeAsync(EnumSubscriptionType.DataChanged, items);
            subscribeTask.Wait();
            SubscriptionResponse subscriptionResponse = subscribeTask.Result;

            if (subscriptionResponse.Error != null)
            {
                Console.WriteLine(string.Format("An error has occurred : {0}", subscriptionResponse.Error));
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
                Console.WriteLine("Error occurred during client creation: {0}", ex.Message);
            }
            Console.WriteLine("");
            return apiClient;
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