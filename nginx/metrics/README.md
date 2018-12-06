# NGINX Metrics

## Introduction

The Lua [nginx-metrics](./metrics/nginx-metrics.lua) script is designed to retrieve the names and values for various NGINX metrics. NGINX's API offers the ability to retrieve information about the server's statuses. This information(metrics) is retrieved via HTTP requests.

The script has two main parts. One part is executed once on an uninitialized machine. This allows the script to retrieve the names of the metrics it is retrieving. The second part is executed repeatedly to obtain the actual data.

This script depends on the following script:

* [esi-lcurl-http-client](https://github.com/inmation/inmation-ESI/tree/master/lib/http/lib)

## Initialization

The initialization is done by 2 recursive function which call 2 other functions. At the top of the script the URL where the NGINX API is exposed is declared. The path where the items should be created in DataStudio is also declared.

A GET request is made to the URL declared at the top of the script. This returns a list containing the top level metric names. each one of these names can be appended to the URL. When a GET request is now sent to the new URL with the added metric name, a table of sub-level names is returned. This sub-level table can either be bottom-level metrics(containing data) or more metric names each with yet another sub-level.

The script passes the table of all metric names to function: "CreateGroups". This uses the Lua inmation.mass() function to create GenFolders for each of the metric names. These GenFolders will contain the metrics. The table of metric names is then passed to the recursive function: "CreateSubLevel". This function does the bulk of the work in the script

### CreateSubLevel

This is a recursive function which requires 3 parameters:

* rootNames
* rootPath
* rootURL

#### rootNames

A table containing all the metric names.

#### rootPath

The root path at which the items should be created in datastudio.

#### rootURL

the root URL to which the script will make HTTP requests.

---

The function takes the metric names in the rootNames table and appends it to both the rootURL and rootPath. It then sends a HTTP request to the new URL and should receive a table from the request. The function then checks if this table contains bottom-level metrics i.e. values or only metric names with more sub-levels. if there are more sub-levels the new URL, path and table are used as parameters and the function is called recursively. If the table contains bottom-level metrics, the inmations library's "Mass" function is used to create Variables for each of the metrics. The function then appends the next metric name in the current rootNames table and repeats this process until each item in the table is handled.

There is a third possible result from the GET request. This is a table which contains sub-levels, but the names cannot be appended the the URL to send a new request. This will not be a valid URL. Therefore a second recursive function: "handleNested" is called. This function goes through the entire table passed to it without any new HTTP requests, creating GenFolders and Variables when appropriate.

Both recursive functions call the 2 functions: "createGroups" and "createVariables" to create GenFolders and Variables respectively. The "createGroups" function requires two parameters:

* A table of names of the metrics for which GenFolders need to be created
* The rootPath at which the GenFolders should be created.

The "createVariables" function requires an extra parameter along with the "createGroups" parameters:

* A table containing the values of bottom-level metrics to assign initial values.

## Data

In order to obtain data from the metrics, there are similar functions as the ones used to initialize, however new Variables or GenFolders are never created, data is just stored in the existing ones. These functions are "getSubLevel", "getNestedValues" and "getVariableValues".