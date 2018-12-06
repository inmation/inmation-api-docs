import pandas as pd
from inmationhttpclient import Client

baseURL = "http://localhost:8002"
options = {
    "auth": {
        "username": '%USERNAME%',
        "password": '%PASSWORD%',
        "authority": 'inmation',
        "grant_type": "password"
    }
}

start_time = "2018-01-01T00:00:00.000Z"
end_time = "2018-02-01T00:00:00.000Z"
intervals_no = 31
items = [
    {
        "p": "%PATH%",
        "aggregate": "AGG_TYPE_INTERPOLATIVE"
    },
]

client = Client(baseURL, options)

def azureml_main(dataframe1 = None, dataframe2 = None):
    r = client.readHistoricalData(items, start_time, end_time, intervals_no)

    print("JSON")
    jsonRes = r.json()
    print(type(jsonRes))
    dataFrameData = {}
    if isinstance(jsonRes, dict):
        print("r.json is an dictionary!!")
        data = jsonRes['data']
        if isinstance(data, dict):
            print("data is an dictionary!!")
            items = data['items']
            if isinstance(items, list):
                print("items is a list!!")
                for item in items:
                    objPath = item['p']
                    itempath = objPath.split('/')
                    objName = itempath[-1]
                    intervals = item['intervals']
                    v = []
                    q = []
                    t = []
                    print(type(v))
                    dataFrameData[objName + '_v'] = v
                    dataFrameData[objName + '_q'] = q
                    dataFrameData[objName + '_t'] = t
                    print(type(v))
                    if isinstance(intervals, list):
                        print("intervals is a list!!")
                        for interval in intervals:
                            v.append(interval['V'])
                            q.append(interval['Q'])
                            t.append(interval['T'])

    print(v, q, t)
    dataframe = pd.DataFrame(data = dataFrameData)
    return dataframe