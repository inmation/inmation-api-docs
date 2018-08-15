# inmation Node.js API Client

The client can be downloaded from [NPM](https://www.npmjs.com/package/@inmation/inmation-api-client).

## Install

```cmd
$ npm install @inmation/inmation-api-client --save
<shows the installed version>
```

## Example DataChange

This example shows a DataChange subscription. When the value changes in system:inmation, `onDataChanged` will be invoked with the changed item values.

items is an array with objects containing:

- v (value)
- q (quality, OPC quality code)
- t (timestamp, ISO 8601 UTC)

Example of items in JSON representation:

```json
[
    {
        "v": 100,
        "q": 0,
        "t": "2018-01-05T22:05:57.618Z"
    }
]
```

Code snippet:

```javascript
const { Client, model } = require('@inmation/inmation-api-client');

const wsURL = 'ws://HOSTNAME:8002/ws';
// For secure connection use wss://HOSTNAME:8002/ws

const options = {
    auth: {
        username: "username@domain.com",
        password: "password",
        authority: "ad",
        grant_type: "password"
    }
};

const client = new Client();

client.onDataChanged((err, items) => {
    if (err) console.log(err.message);
    for (const item of items) {
        console.log(`Path: ${item.p} value: ${item.v}`);
    }
});

client.onError((err) => {
    console.log(`Error: ${err.message}`);
});

client.onWSConnectionChanged((connectionInfo) => {
    console.log(`Connection state: ${connectionInfo.state},
        ${connectionInfo.stateString},
        authenticated: ${connectionInfo.authenticated}`);
});

console.log(`Connect to : ${wsURL}`);
client.connectWS(wsURL, (err) => {
    if (err) return console.log(err.message);

    // Make sure you have some IO, Generic or Data Holder Items in inmation and copy their path(s) over here.
    const items = [
        new model.Item('/System/Core/Simulation/Saw100'),
        new model.Item('/System/Core/Simulation/Saw200')
    ];

    client.subscribeToDataChanges(items, (err) => {
        if (err) console.log(err.message);
    });
}, options);
```