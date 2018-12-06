import json
import requests
import datetime
import pandas as pd

class Client:
    baseURL = "http://localhost:8002"
    options = {
        "auth": {
            "username": '',
            "password": '',
            "authority": 'inmation',
            "grant_type": "password"
        }
    }

    def __init__(self, baseURL, options):
        self.baseURL = baseURL
        self.options = options

    def reqHeaders(self):
        headers = {
            "Content-Type": 'application/json'
        }
        options = self.options or {}
        auth = options["auth"] or {}
        headers["username"] = auth["username"]
        headers["password"] = auth["password"]
        headers["authority"] = auth["authority"]
        headers["grant_type"] = auth["grant_type"]
        return headers

    def createHistoryRequestItem(self, path, aggregate = None):
        agg = aggregate or 'AGG_TYPE_INTERPOLATIVE'
        return {
            "p": path,
            "aggregate": agg
        }

    def createItemValue(self, p, v, q, t):
        return {
            "p": p,
            "v": v,
            "q": q,
            "t": t
        }

    def readHistoricalData(self, items, start_time, end_time, intervals_no):
        reqBody = {
            "start_time": start_time,
            "end_time": end_time,
            "intervals_no": intervals_no,
            "items": items
        }
        headers = self.reqHeaders()
        url = "{}/api/v2/readhistoricaldata".format(self.baseURL)
        print("inmation Web API URL: '{}'".format(url))
        print("inmation Web API headers: '{}'".format(headers))
        print("inmation Web API body: '{}'".format(json.dumps(reqBody)))
        r = requests.post(url, data = json.dumps(reqBody), headers = headers)
        return r

    def write(self, itemvalues):
        reqBody = {
            "items": itemvalues
        }
        headers = self.reqHeaders()
        url = "{}/api/v2/write".format(self.baseURL)
        print("inmation Web API URL: '{}'".format(url))
        print("inmation Web API headers: '{}'".format(headers))
        print("inmation Web API body: '{}'".format(json.dumps(reqBody)))
        r = requests.post(url, data = json.dumps(reqBody), headers = headers)
        return r