# Read Raw Historical Data

- Can return Historical data points or a query strategy.

- Returns by default a query strategy when the total number of data points exceeds three million.

- Can be forced to return less data points by setting the 'query_count_limit'

- Can be used to fetch historical data in reversed order by switching the start and end time.

- Accepts a filter for server side filtering.

- Accepts 'Match Duration' filters to perform server side duration calculations.

## Filtering

The filter option can either be an Aggregation Pipeline with the supported stages '$match' and '$limit' or a match query. The syntax correspond respectively to the [MongoDB’s Aggregation Pipeline](https://docs.mongodb.com/manual/reference/operator/aggregation-pipeline/#aggregation-pipeline-operator-reference) or [read operation query](https://docs.mongodb.com/manual/tutorial/query-documents/#read-operations-query-argument) syntax.

Supported Pipeline Stages

| Stage | Description |
|-------|-------------|
| $match | Filters the historical data points. |
| $limit | Limits the number of matching historical data points. The first *n* data points will be returned, where *n* is the specified limit. |

Supported query selectors are:

### Comparison

| Name | Description |
|------|-------------|
| $eq | Matches values that are equal to a specified value. |
| $gt | Matches values that are greater than a specified value. |
| $gte | Matches values that are greater than or equal to a specified value. |
| $in | Matches any of the values specified in an array. |
| $lt | Matches values that are less than a specified value. |
| $lte | Matches values that are less than or equal to a specified value. |
| $ne |  Matches all values that are not equal to a specified value. |
| $nin |  Matches none of the values specified in an array. |

### Logical

| Name | Description |
|------|-------------|
| $and | Joins query clauses with a logical AND returns all documents that match the conditions of both clauses. |
| $or | Joins query clauses with a logical OR returns all documents that match the conditions of either clause. |

## Duration calculations

With the field option 'd' it is possible to request the server to calculate the duration between each historical data point. The first duration is calculated from the interval start time, the last duration is calculated till the end time of the requested interval.

```ascii
Interval                         Interval
 Start                             End
   +                                +
40 |            x                   |
30 |                          x     |
20 |                   x            |
10 |     x                          |
   +---------------------------------

   <----> <----> <----> <----> <---->
    d[1]   d[2]   d[3]   d[4]   d[5]


```

### Match Duration

The 'Match Duration' feature gives the possibility to perform (server-side) duration calculations based on a match expression. To be able to calculate the duration for the first and last data point in the requested interval the first matching bounding values will automatically be fetched based on the provided Match duration options:

| Name | Description |
|-------|-------------|
| leading_bounding_time_span | The maximum time span in milliseconds before the leading bounding value of the requested interval to search for a matching bounding value. |
| leading_bounding_intervals_no | The number of intervals to use to search for a matching leading bounding value. |
| trailing_bounding_time_span | The maximum time span in milliseconds after the trailing bounding value of the requested interval to search for a matching bounding value. |
| trailing_bounding_intervals_no| The number of intervals to use to search for a trailing leading bounding value. |

The following example shows how Match Durations can be defined in a 'ReadRawHistoricalDataQuery' / request body:

```json
{
    "md": [
        {
            "name": "MyDuration01",
            "expression": {
                "v": {
                    "$gt": 20
                }
            },
            "leading_bounding_time_span": 15000,
            "trailing_bounding_time_span": 15000,
            "leading_bounding_intervals_no": 3,
            "trailing_bounding_intervals_no": 3
        }
    ]
}
```

For example, setting the 'leading_bounding_time_span' 15000 and the 'leading_bounding_intervals_no' to 3, means the logic searches for a matching leading bounding value in three steps starting from the timestamp of the leading bounding value of the requested interval.

```ascii
     matching
     leading                              interval
     bounding                    leading   start
      value                      bounding    +
        x                         value      |
                                    .<------>+
                                      3000ms |
                                             |
      <------------------------------------->+---+
          leading bounding time span

      <----><----------><---------->
     2000ms  5000ms      5000ms
(interval 3)(interval 2)(interval 1)
```

Data point 'x' is included in the response and used in the match duration calculation.
Data point '.' is excluded from the response and match duration calculation.

#### Examples

The following example shows the output based on the Match Duration expression '{"v" : {"$gt": 20}}':

```ascii
        interval                         interval
         start                             end
           +                                +
x       40 |            x                   |        x
        30 |                          x     |
     *  20 |                   *            |  *
        10 |     *                          |
           +--------------------------------+

 <---------------------> <-----------> <------------>
  d[1]                    d[2]          d[3]
```

Data points indicated by '*' are included in the response but excluded from the match duration calculation.

Match Duration calculations can be used together with the filter option. The following example shows the output based on the Match Duration expression '{"v" : {"$gt": 20}}' and filter '{"v" : {"$gte": 20}}'

```ascii
        interval                         interval
         start                             end
           +                                +
x       40 |            x                   |        x
        30 |                          x     |
     *  20 |                   *            |  *
        10 |     .                          |
           +--------------------------------+

 <---------------------> <-----------> <------------>
  d[1]                    d[2]          d[3]
```

### Post Execution

The Post execution feature makes it possible to execute any Lua logic server-side (in the Core) to process / transform the data returned by a 'ReadRawHistoricalData' query.

The following example shows how Post Execution functions can be defined in a 'ReadRawHistoricalDataQuery' / request body:

```json
{
    "post_execution": [
    {
        "lib": "CalcLib",
        "func": "countOccurrences",
        "arg": {}
    }
  ]
}
```

The library defined in a Post Execution item must be available in the context of the Web API. An example library to count the number of historical data points per item can be found [here](./scripts/rrhd-post-exec.lua)

The implementation of the library

```lua
local lib = {}

function lib:countOccurrences(arg)
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
```

#### Function argument

The raw historical data query result gets passed to the function as part of the `arg` argument. The JSON representation is as follows:

```json
{
    "data": {
        "query_index": "1",
        "start_time": "2018-11-11T00:00:00.000Z",
        "end_time": "2018-11-13T00:00:00.000Z",
        "items": [
            {
            "p": "/System/Core/...",
            "v": [],
            "q": [],
            "t": []
            }
        ]
    }
}
```

#### Response content type

Post Execution makes it also possible to return the data in a different format then JSON. The example library [rrhd-post-exec-with-headers](./scripts/rrhd-post-exec-with-headers.lua) show how to return a csv by defining the HTTP headers `'["Content-Type"] = "txt/csv"'` in the response.

#### Function chain

By defining multiple 'execFunction' items in the 'post_execution' array. A chain of functions can be created. During execution the result of a function will be passed to the next function in the `data` field as part the `arg` argument.