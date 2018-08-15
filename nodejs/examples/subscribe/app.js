'use strict';

const { Client, model } = require('@inmation/inmation-api-client');

const wsURL = 'ws://HOSTNAME:8002/ws';
const options = {
    auth: {
        username: "<USERNAME>",
        password: "<PASSWORD>",
        authority: "inmation",
        grant_type: "password"
    }
};

const processItems = (items) => {
    if (Array.isArray(items)) {
        for (const item of items) {
            if (item.error) {
                console.log(`Path: ${item.p} error: ${item.error.msg}`);
            }
            else {
                console.log(`Path: ${item.p} value: ${item.v}`);
            }
        }
    }
}

const client = new Client();

client.onDataChanged((err, items) => {
    if (err) console.log(`onDataChanged error: ${err.message}`);
    processItems(items);
});

client.onError((err) => {
    console.log(`Error: ${err.message}`);
});

client.onWSConnectionChanged((connectionInfo) => {
    console.log(`Connection state: ${connectionInfo.state}, ${connectionInfo.stateString}, authenticated: ${connectionInfo.authenticated}`);
});

console.log(`Connect to : ${wsURL}`);
client.connectWS(wsURL, (err) => {
    if (err) return console.log(err.message);

    // Make sure you have some IO, Generic or Data Holder Items in inmation and copy their path(s) over here.
    const items = [
        new model.Item('/System/Core/Simulation/Saw100'),
        new model.Item('/System/Core/Simulation/Saw200')
    ];

    client.subscribeToDataChanges(items, (err, items) => {
        if (err) return console.log(err.message);
        processItems(items);
    });
}, options);