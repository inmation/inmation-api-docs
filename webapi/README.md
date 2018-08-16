# inmation Web API

The system:inmation Web API (Application Programming Interface) is hosted in a Windows Service. It can be used by any external application as an interface to system:inmation, using the HTTP or WebSocket Interface. 

The Web API is also used extensively by Visual KPI in enterprise:inmation, to provide and update the data items displayed on the Web dashboard.

## Installation & Execution

There are different ways to install the Web API service. First, the service may be installed using the system:inmation Windows Installer (inmatonSetup.exe). The service may also be installed using command line arguments. Supported command line arguments are described in the following section.

### Command line arguments

| Action | Options | Description |
| ------ | --------- |------------ |
| help | | Shows the help page. |
| | | |
| -i | | Install the inmation Web API as a service.|
| | --corehost | (Default: localhost) Specifies the Core's host address.|
| | --coreport | (Default: 6512) Specifies a particular setting for the Core Service Port.|
| | --profile | API User profile which is used in the communication with the Core. |
| | --pwd | API User password which is used in the communication with the Core. |
| | | |
| -e | | Executes the Web API as a console application. |
| | --service | (Default: 0) Value 1 indicates the inmation Web API is started as a Windows Service. |
| | --oid | Specifies the inmation 'Web API Server' object ID. |
| | --auth | Contains the credentials for the inmation Web API account. (used when installed as a service.) |
| | --corehost | (Default: localhost) Specifies the Core's host address. |
| | --coreport | (Default: 6512) Specifies a particular setting for the Core Service Port. |
| | --profile | API User profile which is used in the communication with the Core. |
| | --pwd | API User password which is used in the communication with the Core. |
| | | |
| -r | | Removes the inmation Web API Windows service. |

After installation a Web API Server Object is automatically created in the inmation Server Model. Only the first Web API Server object in the Server Model will be automatically be enabled after creation.

## Settings

Settings are stored in the Web API Server Object in the inmation Server model.

The file 'inmation.WebApi.WindowsService.exe.config' contains settings to be able to connect and the HTTP base address, which is only used during setup and creation of the Web API Server Object.

## Swagger

The Swagger documentation can be viewed in de Web Browser by visiting: `http://hostname:8002/api/docs`.

## Authentication

The Web API supports token authentication based on inmation profiles and Windows domain and local accounts. More detailed information can be found [here](./authentication.md).

## HTTPS - WSS

Both interface can have encryption with TLS/SSL. See instructions [here](./encryption.md).

## Features

### ExecFunction

ExecFunction feature makes it possible to create 'custom endpoints'. Request can be send to the server containing an object (Lua Table) as an argument. Response can be anything in which an object (Lua table) is automatically converted to JSON.

Make sure the function within the library is declared with a colon notation.

In Lua: `lib:myfunction(arg)` is the same as `lib.myfunction(self, arg)`.

Lua script example:

```lua
local lib = {}

function lib:say_hello(arg)
    local _arg = arg or {} -- in case no argument is set
    local name = _arg.name or 'unknown'
    local msg = string.format("Hello %s", name)
    return msg
end

return lib
```

This feature can be tested with Swagger. In case the library is stored in a different hierarchical scope then the context path defined in the Web API server object, you need to set the context `cxt` in the request.