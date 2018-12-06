-- ReadRawHistoricalData Post Execution example.
-- Creating custom response headers.
return function(arg, _, hlp)
    arg = arg or {}
    local result = [[Path,Count]]
    local data = arg.data or {}
    for _, item in ipairs(data.items) do
        local count = 0
        -- v can contain nil values, use t.
        if type(item.t) == 'table' then
            count = #item.t
        end
        result = ("%s\n\r%s,%s"):format(result, item.p, count)
    end
    local headers = {
        ["Content-Type"] = "txt/csv"
    }
    return hlp:createResponse(result, nil, 200, headers)
end