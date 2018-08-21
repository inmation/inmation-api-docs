# Execute Function

- Execute Function feature makes it possible to create 'custom endpoints'.
- Power by Lua Library scripts.

Request can be send to the server containing an object (Lua Table) as an argument. Response can be anything in which an object (Lua table) is automatically converted to JSON.

## Lua script example

```lua
local lib = {}

function lib.say_hello(_, arg)
    local _arg = arg or {} -- in case no argument is set
    local name = _arg.name or 'unknown'
    local msg = string.format("Hello %s", name)
    return msg
end

return lib
```

Make sure the function within the library is declared with a colon notation.

In Lua: `lib:myfunction(arg)` is the same as `lib.myfunction(self, arg)`.

This feature can be tested with Swagger. In case the library is stored in a different hierarchical scope then the context path defined in the Web API server object, you need to set the context `cxt` in the request.

## Arguments

| Name | Description |  
|------|-------------|
| self | The Lua 'self' parameter. |
| arg | The provided 'farg' parameter in the request as a Lua table. |
| req | A Lua table which contains the request information, like HTTP headers and access token payload. (Available since Web API version 1.38.x) |
| helper | A proxy object which contains convenience functions. (Available since Web API version 1.38.x)|

The available helper functions are described in more detail in the following sections.

### Checkpermission Helper

The 'checkpermission' helper function is a convenience function to check whether the user, who executes the request, has permission based on the provided access token:

- checkpermission(pathspec, sec_attr)
  - Parameters
    - pathspec - this parameter can be either a string or table, representing the object's path, or the inmation object itself, or the numeric object or property id
    - sec_attr - a bitwise OR combination of SecurityAttributes flags

Example:

```lua
local lib = {}
function lib.checkpermissionExample(_, arg, req, helper)
    local _arg = arg or {}
    local result = {}
    result.arg = arg
    result.req = req

    local pathSpec = _arg.pathspec or '/System'
    local checkpermissionRes = helper:checkpermission(pathSpec, inmation.model.flags.SecurityAttributes.READ)
    if not checkpermissionRes.granted then
        return checkpermissionRes:responsevqt()
    end

    result.granted = true
    return result
end

return lib
```

### Create Response Helper

The 'createResponse' helper function can be used to set the value, error and HTTP status code, which should be returned by the Web API. This function creates a ‘inmation RunScript Response’ object, which will only be inspected by the Web API (.NET code) in case the quality returned by the Lua script is not equal to zero (GOOD).

- createResponse(data, error, status_code)

Example:

```lua
local lib = {}

function lib.errorResponseStatusCodeExample(_, _, _, helper)
    local data = nil -- this example does not return any data.
    local err = {
        msg = 'Custom error message',
        code = 123, -- custom error code.
        loremIpsum = "Lorem Ipsum" -- custom error field.
    }
    local status_code = 503
    return helper:createResponse(data, err, status_code)
end

return lib
```