# inmation Web API

The system:inmation Web API (Application Programming Interface) is hosted in a Windows Service. It can be used by any external application as an interface to system:inmation, using the HTTP or WebSocket Interface.

The Web API is also used extensively by Visual KPI in enterprise:inmation, to provide and update the data items displayed on the Web dashboard.

## Installation & Execution

There are different ways to install the Web API service. First, the service may be installed using the system:inmation Windows Installer (inmatonSetup.exe). The service may also be installed using command line arguments. Supported command line arguments are described in the following section.

### Command line arguments

| Action | Options | Description |
| ------ | --------- |------------ |
| help | | Provides information about available actions and options. |
| | | |
| -i | | Install the inmation Web API as a service.|
| | corehost | Specifies the Core's host address, by default localhost|
| | coreport | Specifies a particular setting for the Core Service Port, by default 6512.|
| | instance | Optional, specifies the instance name for the inmation Web API. |
| | profile | API User profile which is used in the communication with the Core. |
| | pwd | API User password which is used in the communication with the Core. |
| | | |
| -e | | Executes the Web API as a console application. |
| | auth | Contains the credentials for the inmation Web API account. (used when installed as a service.) |
| | corehost | Specifies the Core's host address, by default localhost|
| | coreport | Specifies a particular setting for the Core Service Port, by default 6512.|
| | oid | Specifies the inmation 'Web API Server' object ID to link to. |
| | profile | API User profile which is used in the communication with the Core. |
| | pwd | API User password which is used in the communication with the Core. |
| | service | (Default: 0) Value 1 indicates the inmation Web API is started as a Windows Service. |
| | | |
| -r | | Removes the 'inmation Web API' Windows service. |
| | instance | Optional, specifies the instance name of the Windows service to remove, by default "inmation Web API" |

Command line options have the prefix '--'.

Example command to install the inmation Web API as a Windows service:

```txt
 inmationWebAPI.exe -i --corehost localhost --coreport 6512 --profile webapi --pwd password
 ```

After installation a Web API Server Object is automatically created in the inmation Server Model. Only the first Web API Server object in the Server Model will automatically be enabled after creation.

## Settings

Settings are stored in the Web API Server Object in the inmation Server model.

The file 'inmation.WebApi.WindowsService.exe.config' contains settings to be able to connect and the HTTP base address, which is only used during setup and creation of the Web API Server Object.

## Swagger

The Swagger documentation can be viewed in a Web Browser by visiting: `http://hostname:8002/api/docs`.

## Authentication

The Web API supports token authentication based on inmation profiles and Windows domain and local accounts. More detailed information can be found [here](./authentication.md).

## HTTPS - WSS

Both interface can have encryption with TLS/SSL. See instructions [here](./encryption.md).

## Features

### [Execute Function](execfunction.md)

### [Read Raw Historical Data](readrawhistoricaldata.md)