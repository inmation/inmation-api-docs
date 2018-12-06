local HTTPClient = require('esi-lcurl-http-client')
local httpClient = HTTPClient.new()
local base = "/System/Core/Performance Counter/NGINX"
local url = "http://username:password@hostname:port/api/3"

local initialized = false

local function createGroups(groupNames, rootPath)
	local groups = {}
	for i = 1, #groupNames do
		table.insert(groups,
		{
			class = inmation.model.classes.GenFolder,
			operation = inmation.model.codes.MassOp.UPSERT,
			path = ("%s/%s"):format(rootPath, groupNames[i]),
			ObjectName = groupNames[i]
		})

	end
	inmation.mass(groups, 0)
end

local function createVariables(variableNames, variableValues, rootPath)
	local variables = {}
	for i = 1, #variableNames do
		table.insert(variables,
		{
			class = inmation.model.classes.Variable,
			operation = inmation.model.codes.MassOp.UPSERT,
			path = ("%s/%s"):format(rootPath, variableNames[i]),
			ObjectName = variableNames[i],
			["ArchiveOptions.StorageStrategy"] = "STORE_RAW_HISTORY"
		})
	end
	inmation.mass(variables, 0)
	for i = 1, #variableNames do
		local path = ("%s/%s"):format(rootPath, variableNames[i])
		inmation.setvalue(path, variableValues[i])
	end
end

local function handleNested(nestedTable, rootNames, rootPath)
	for i = 1, #rootNames do

		--since the nested tables can have both variables and tables in the same layer
		--we have to check the type of each item and sort them accordingly
	    local variableKeys = {}
		local tableKeys = {}
		local metricVariables = {}
		local metricTables = {}

		--fill metrics with the body of the http request
		for k, v in pairs(nestedTable[i]) do
			if (type(v) == "table") then
				table.insert(metricTables, v)
				table.insert(tableKeys, k)
			else
				table.insert(metricVariables, v)
				table.insert(variableKeys, k)
			end
	    end
		--update the path at which the items will be created
		local path = ("%s/%s"):format(rootPath, rootNames[i])

		--if the items are bottom level, create variables for each item
		if (#metricVariables > 0) then
			createVariables(variableKeys, metricVariables, path)
		end
		--if the items are tables, create directories and call the function recursively
		if (#metricTables > 0) then
			createGroups(tableKeys, path)
			handleNested(metricTables, tableKeys, path)
		end
    end
end

local function createSubLevel(rootNames, rootPath, rootURL)
	--for each endpoint
    for i = 1, #rootNames do
		--update the URL for the sublayer to be retrieved
		local route = ("%s/%s"):format(rootURL, rootNames[i])
		--get the table at this URL
		local metrics = httpClient:GET(route)

		--create arrays to store table information
		--the keys will be strings for bottom level
	    local metricKeys = {}
		--the values will be strings for nested tables
		local metricValues = {}

		--if the request is ok
		if (metrics.code == 200) then

			--isolate the keys and values.
			--this allows us to determine the type of objects we are dealing with
		    for k, v in pairs(metrics.data) do
	            table.insert(metricKeys, k)
				table.insert(metricValues, v)
	        end

			--update the path for creating directories/variables
			local path = ("%s/%s"):format(rootPath, rootNames[i])

			--nested tables without additional http layers
			if (type(metricValues[1]) == "table") then
				--create directories for the top level of the nested tables
				createGroups(metricKeys, path)
				--recursive function to map nested tables
				handleNested(metricValues, metricKeys, path)
		    --bottom level
			elseif (type(metricKeys[1]) == "string") then
				--since this is bottom level, just create variables
			    createVariables(metricKeys, metricValues, path)
		    --nested tables with additional http requests required
			else
				--create directories for the current top level
				createGroups(metricValues, path)
				--since we require additional http requests, call this function again
				--the current path and url are set as the new root in the recursion
			    createSubLevel(metricValues, path, route)
		    end
		end
    end
end

local function getVariableValues(variableNames, variableValues, rootPath)
	for i = 1, #variableNames do
		local path = ("%s/%s"):format(rootPath, variableNames[i])
		inmation.setvalue(path, variableValues[i])
	end
end

local function getNestedValues(nestedTable, rootNames, rootPath)
	for i = 1, #rootNames do

		--since the nested tables can have both variables and tables in the same layer
		--we have to check the type of each item and sort them accordingly
	    local variableKeys = {}
		local tableKeys = {}
		local metricVariables = {}
		local metricTables = {}

		--fill metrics with the body of the http request
		for k, v in pairs(nestedTable[i]) do
			if (type(v) == "table") then
				table.insert(metricTables, v)
				table.insert(tableKeys, k)
			else
				table.insert(metricVariables, v)
				table.insert(variableKeys, k)
			end
	    end
		--update the path at which the items will be created
		local path = ("%s/%s"):format(rootPath, rootNames[i])

		--if the items are bottom level, create variables for each item
		if (#metricVariables > 0) then
			getVariableValues(variableKeys, metricVariables, path)
		end
		--if the items are tables, create directories and call the function recursively
		if (#metricTables > 0) then
			getNestedValues(metricTables, tableKeys, path)
		end
    end
end

local function getSubLevel(rootNames, rootPath, rootURL)
	--for each endpoint
    for i = 1, #rootNames do
		--update the URL for the sublayer to be retrieved
		local route = ("%s/%s"):format(rootURL, rootNames[i])
		--get the table at this URL
		local metrics = httpClient:GET(route)

		--create arrays to store table information
		--the keys will be strings for bottom level
	    local metricKeys = {}
		--the values will be strings for nested tables
		local metricValues = {}

		--if the request is ok
		if (metrics.code == 200) then

			--isolate the keys and values.
			--this allows us to determine the type of objects we are dealing with
		    for k, v in pairs(metrics.data) do
	            table.insert(metricKeys, k)
				table.insert(metricValues, v)
	        end

			--update the path for creating directories/variables
			local path = ("%s/%s"):format(rootPath, rootNames[i])

			--nested tables without additional http layers
			if (type(metricValues[1]) == "table") then
				--recursive function to map nested tables
				getNestedValues(metricValues, metricKeys, path)
		    --bottom level
			elseif (type(metricKeys[1]) == "string") then
				--since this is bottom level, just create variables
			    getVariableValues(metricKeys, metricValues, path)
		    --nested tables with additional http requests required
			else
				--since we require additional http requests, call this function again
				--the current path and url are set as the new root in the recursion
			    getSubLevel(metricValues, path, route)
		    end
		end
    end
end

return function()
	if (initialized == false) then
		--get the top layer of endpoints
		local rootEndpoints = httpClient:GET(url)
		local topLevel = {}

		--isolate the names of each endpoint
		for _, v in pairs(rootEndpoints.data) do
			table.insert(topLevel, v)
		end

		--create directories for each endpoint using the names
		createGroups(topLevel, base)
		--create sub-directories for each endpoint using the names.
		--pass the base path and base URL so that the recursive function can update them going forward
		createSubLevel(topLevel, base, url)
		initialized = true
	end
	if (initialized == true) then
		--get the top layer of endpoints
		local rootEndpoints = httpClient:GET(url)
		local topLevel = {}

		--isolate the names of each endpoint
		for _, v in pairs(rootEndpoints.data) do
			table.insert(topLevel, v)
		end

		--create sub-directories for each endpoint using the names.
		--pass the base path and base URL so that the recursive function can update them going forward
		getSubLevel(topLevel, base, url)
	end
return initialized

end