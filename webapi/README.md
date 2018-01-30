# inmation Web API

## Application settings

The file 'inmation.WebApi.WindowsService.exe.config' contains the application settings.

`inmation.Api.ApplicationContextPath`

The application context path must be an existing folder in the system. This folder can then be used to store Lua library scripts to be executed via ExecFunction.

Web API metrics will also be stored in this folder. See section 'MetricsAggregationInterval'.

Best practice for creating an application context path is the following structure:

hierarchy | type
--------- | ----
/System | System
-- /Core | Core
-- -- /APIContext | Folder
-- -- -- /WebAPI(instance number) | Folder

```xml
<add key="inmation.Api.ExecFunction.AllowAnonymous" value="/System/Core/APIContext/WebAPI01" />
```

`MetricsAggregationInterval`

The Web API is able to collect and store performance metrics. The setting 'inmation.Api.MetricsAggregationInterval' holds the interval in seconds to aggregate the metrics and store them in the system. Works only in case an ApplicationContextPath is provided. Interval value of 0 indicates the aggregation of metrics is disabled.

```xml
<add key="inmation.Api.MetricsAggregationInterval" value="5" />
```

For additional settings read the README.md in the installation folder of this service.

## Swagger

The Swagger documentation can be viewed in de Web Browser by visiting: `http://hostname:8002/api/docs`.

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

This feature can be tested with Swagger. In case the `ApplicationContextPath` is not set or the library is stored in a different hierarchical scope, you need to set the context `cxt` in the request.