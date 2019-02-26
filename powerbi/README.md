# inmation Power BI Connector

Below, instructions for installing and working with inmation Power BI connectors.

## Installation

To download and install the connector:

1. In **This PC / Documents** directory create a folder **Power BI Desktop** inside create another new folder and name it **Custom Connectors**.
2. Download the connector file(s).
3. Place the file into the **Documents/Power BI/Custom Connectors** folder.

To avoid a warning about connection to the third-party services, you will need to enable loading uncertified connectors:

1. Open Power BI Desktop.
2. Go to File | Options and settings | Options.
3. Go the Security tab.
4. Under Data Extensions, select Allow any extension to load without warning or validation.
5. Restart Power BI Desktop.

## Connecting

Open a new Power BI file, go to the options and settings, in the **Preview features** tab enable **Custom data connectors** by selecting it. This feature allows the data structure preview when loading the data.

Open **Get Data** and search for inmation connectors. Pick one of the following connectors:

1. [**inmation - Read Historical Data**](./inmationReadHistoricalData.mez) connector. Returns specified object's processed historical data.
2. [**inmation - Execute Function**](./inmationExecFunction.mez) connector. Fetches data by using the inmation Web API [ExecuteFunction](../webapi/execfunction.md) endpoint.

## Arguments

Depending on which connector you are going to use, you will see one of the following argument tables in the first display screen. Below tables contain explanation of each argument.

### inmation - Read Historical Data

| Name | Description |  
|------|-------------|
| Web API base URL | The base address of the inmation Web API. |
| Start Time | The start time (UTC) of the interval to retrieve data for. |
| End Time | The end time (UTC) of the interval to retrieve data for. |
| Paths | Comma separated paths of the object to be queried for historical data. |
| Aggregate | The aggregate type as string code. E.g. Average |

### inmation - Execute Function

| Name | Description |  
|------|-------------|
| Web API base URL | The base address of the inmation Web API.|
| Library | Name of the script library which contains or returns the function to execute. |
| Function (optional) | Name of the function to execute. Required in case the library itself does not return a function. |
| Context (optional) | A path, which specifies the context in which the provided function has to be executed. By default, the context path of the inmation Web API server object.|

Second display screen is a login screen containing your inmation **username** and **password** input fields.