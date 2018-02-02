# inmation .Net API Client

The client can be downloaded from [NuGet](https://www.nuget.org/packages/inmation-api-client).

## Example Console application

- Create a Console application.
- Add the 'inmation-api-client' NuGet package to the project.
- Replace the content of the Program.cs class with the one of the following examples:
    * [Read & Write](https://github.com/inmation/inmation-api-docs/tree/master/dotnet/src/read-write-example.cs)
    * [Subscribe](https://github.com/inmation/inmation-api-docs/tree/master/dotnet/src/subscribe-example.cs)
    * [ReadRawHistoricalData](https://github.com/inmation/inmation-api-docs/tree/master/dotnet/src/readrawhistoricaldata-example.cs)

Code snippet:

```csharp
using inmation.api;

Client apiClient = new Client();

string WebSocketUrl = "ws://localhost:8000/ws";

RpcOptions options = new RpcOptions();
options.Username = "USERNAME";
options.Password = "PASSWORD";

// Connect and authenticate. By providing credentials to the connectWs method, the credentials will be stored in the session.
ConnectionResponse connectionresponse = apiClient.ConnectWs(WebSocketUrl, options).Result;
if (connectionresponse.Error != null)
{
    Console.WriteLine(string.Format("Connect failed: {0}", connectionresponse.Error?.First().Message));
}
```