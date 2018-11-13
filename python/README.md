# inmation Python API Client

The client can be downloaded from the Python Package Index [PyPI](https://pypi.org/project/inmation-api-client).

Additional documentation can be found on the [inmation Wiki](https://inmation.com/wiki/Sysdoc/Python_API_Client).

## Install

```cmd
$ pip install inmation-api-client
<shows the installed version>
```

## Example DataChange

This example shows a DataChange subscription. When the value changes in system:inmation, `onDataChanged` will be invoked with the changed item values.

items is an array with objects containing:

- p (Path)
- v (value)
- q (quality, OPC quality code)
- t (timestamp, ISO 8601 UTC)

Example of items in JSON representation:

```json
[
    {
        "p": "/System/Core/Temp",
        "v": 100,
        "q": 0,
        "t": "2018-01-05T22:05:57.618Z"
    }
]
```

Code snippet:

```python
import asyncio
import time
from random import randint
from inmation_api_client import Client, Options, Item, ItemValue

USERNAME = 'name'
PASSWORD = 'password'
WS_URL = "ws://127.0.0.1:8002/ws"

def create_api_client(ioloop):
    client = Client(ioloop)

    def on_connection_changed(conn_info):
        print('Connection state: {}, {}, authenticated: {}'.format(conn_info.state, conn_info.state_string, conn_info.authenticated))
    client.on_ws_connection_changed(on_connection_changed)

    def on_error(err):
        if err:
            print("Error: {}".format(err.message))
    client.on_error(on_error)

    ioloop.run_until_complete(client.connect_ws(WS_URL, Options(USERNAME, PASSWORD)))
    return client

io_loop = asyncio.get_event_loop()
client = create_api_client(io_loop)

async def subscribe_to_data_changes(items):
    print("Subscribing to {} items".format(len(items)))
    def on_data_changed(*args):
        err = args[0]
        if err:
            print(err.message)
        else:
            _items = args[1]
            if isinstance(_items, list):
                for item in _items:
                    item_val = item['v'] if 'v' in item else 'No Value'
                    itempath = (item['p']).split('/')
                    objName = itempath[-1]
                    print("{} - {}".format(objName, item_val))
    client.on_data_changed(on_data_changed)
    await client.subscribe(items, SubscriptionType.DataChanged)

items_path = "/System/Core/Trial/"
items = [Item(items_path + n) for n in ['Humidity', 'LED1', 'LED2', 'Temperature']]

client.run_async([
    subscribe_to_data_changes(items)
])

io_loop.run_forever()
io_loop.close()
```