import pandas as pd
from inmationhttpclient import Client
import json

baseURL = "http://localhost:8002"
options = {
    "auth": {
        "username": '%USERNAME%',
        "password": '%PASSWORD%',
        "authority": 'inmation',
        "grant_type": "password"
    }
}

client = Client(baseURL, options)
p = "%PATH%"

def azureml_main(dataframe1 = None, dataframe2 = None):
    colnames = dataframe1.columns
    scores = dataframe1[colnames[6]]
    timestamps = dataframe1[colnames[0]]
    itemValues = []
    for index, timestamp in timestamps.iteritems():
        v = int(scores[index])
        t = int(timestamp)
        itemValues.append(
            {
                "p": p,
                "v": v,
                "t": t
            } 
        )

    print(itemValues)
    r = client.write(itemValues)
    print(r.json())
    return dataframe1