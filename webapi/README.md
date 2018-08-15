# inmation Web API

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

This feature can be tested with Swagger. In case the `ApplicationContextPath` is not set or the library is stored in a different hierarchical scope, you need to set the context `cxt` in the request.