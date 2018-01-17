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