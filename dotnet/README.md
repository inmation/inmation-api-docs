# inmation .Net API Client

The client can be downloaded from [NuGet](https://www.nuget.org/packages/inmation-api-client).

## Example Console application

- The following example console application projects are available:
    * [Read & Write](https://github.com/inmation/inmation-api-docs/tree/master/dotnet/src/ReadWrite)
    * [Subscribe](https://github.com/inmation/inmation-api-docs/tree/master/dotnet/src/Subscribe)
    * [ReadRawHistoricalData](https://github.com/inmation/inmation-api-docs/tree/master/dotnet/src/ReadRawHistoricalData.MatchDuration)
    * [ReadRawHistoricalData MatchDuration](https://github.com/inmation/inmation-api-docs/tree/master/dotnet/src/ReadRawHistoricalData)
- After downloading the [example solution](https://github.com/inmation/inmation-api-docs/tree/master/dotnet/src/inmation.api.client.example.sln) or one of the console application projects the 'inmation-api-client' NuGet package needs to be restored.

Code snippet:

```csharp
using inmation.api;

Client apiClient = new Client();

string WebSocketUrl = "ws://localhost:8002/ws";
ConnectOptions options = new ConnectOptions("USERNAME", "PASSWORD");

// Connect and authenticate. By providing credentials to the connectWs method, the credentials will be stored in the session.
ConnectionResponse connectResponse = apiClient.ConnectWs(WebSocketUrl, options).Result;
if (connectResponse.Error != null)
{
    Console.WriteLine(string.Format("Connect failed: {0}", connectResponse.Error?.First().Message));
}
```