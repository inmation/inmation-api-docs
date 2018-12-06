-- ReadRawHistoricalData Post Execution example.
local lib = {}

function lib.countOccurrences(_, arg)
    arg = arg or {}
    local result = {}
    local data = arg.data or {}
    for _, item in ipairs(data.items) do
        local count = 0
        -- v can contain nil values, use t.
        if type(item.t) == 'table' then
            count = #item.t
        end
        table.insert(result, {
            p = item.p,
            v = count
        })
    end
    return result
end

return lib