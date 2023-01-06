import {HubConnectionBuilder} from '@microsoft/signalr'


const connection = new HubConnectionBuilder().withUrl("https://localhost:7104/board").build()

console.log("ben sinyalim")

async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.onclose(async () => {
    await start();
});

// Start the connection.
start();

export default connection