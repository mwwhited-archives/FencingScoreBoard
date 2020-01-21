window.ScoreMachineHub = window.ScoreMachineHub || {
    connection: {}
};
window.ScoreMachineVM = window.ScoreMachineVM || {};

function sendData(message) {
    console.log("sendData::".concat(JSON.stringify(message)));
    connection = window.ScoreMachineHub.connection;
    if (connection) {
        connection.invoke("sendData", message)["catch"](function (err) {
            return console.error(err.toString());
        });
    }
}

function startBout() {
    sendData({
        "messageType": "SpecialAction",
        "Action": "StartBout"
    });
}
function endBout() {
    sendData({
        "messageType": "SpecialAction",
        "Action": "EndBout"
    });
}
function setChannel(channel) {
    sendData({
        "messageType": "SpecialAction",
        "Channel": channel || "RESET"
    });
}

function updatePlayers(playerRight, playerLeft) {
    p1 = playerRight || {};
    p2 = playerLeft || {};

    sendData({
        "messageType": "ScoreMachine",

        "playerRightName": (p1.name || "").substring(0, 19),
        "playerRightClub": (p1.club || "").substring(0, 32),

        "playerLeftName": (p2.name || "").substring(0, 19),
        "playerLeftClub": (p2.club || "").substring(0, 32)
    });
}
function overlayOn() {
    sendData({
        "messageType": "ScoreMachine",
        "content": "block"
    });
}
function overlayOff() {
    sendData({
        "messageType": "ScoreMachine",
        "content": "none"
    });
}

function documentReady(startUrl) {
    console.log("documentReady");

    var supportsWebSockets = 'WebSocket' in window || 'MozWebSocket' in window;
    console.log('supportsWebSockets: ' + supportsWebSockets);
    console.log('userAgent: ' + navigator.userAgent);

    var useTransports = signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling | signalR.HttpTransportType.ServerSentEvents;
    if (navigator.userAgent.indexOf("NeTVBrowser") > -1) {
        useTransports = signalR.HttpTransportType.LongPolling | signalR.HttpTransportType.ServerSentEvents;
    }

    var connection = new signalR.HubConnectionBuilder().withUrl("".concat(startUrl || "", "/ScoreMachineHub"), { transport: useTransports }).configureLogging(signalR.LogLevel.Information).build();

    window.ScoreMachineHub.connection = connection;

    window.ScoreMachineHub.sendData = sendData;
    window.ScoreMachineHub.startBout = startBout;
    window.ScoreMachineHub.endBout = endBout;
    window.ScoreMachineHub.updatePlayers = updatePlayers;
    window.ScoreMachineHub.overlayOn = overlayOn;
    window.ScoreMachineHub.overlayOff = overlayOff;
    window.ScoreMachineHub.setChannel = setChannel;

    if (window.ScoreMachineVM && window.ScoreMachineVM.onDocumentReady) {
        window.ScoreMachineVM.onDocumentReady(connection);
    }

    connection.on("ReceiveData", function (message) {
        console.log('ReceiveData: ' + JSON.stringify(message));
        if (message && window.ScoreMachineVM && window.ScoreMachineVM.onReceiveData) {
            window.ScoreMachineVM.onReceiveData(message, connection);
        }
    });

    function startConnection() {
        connection.start().then(function () {
            connection.invoke("sendData", {
                "messageType": "ClientConnected",
                "user-agent": navigator.userAgent
            })["catch"](function (err) {
                return console.error(err.toString());
            });
        })["catch"](function (err) {
            console.error(err.toString());
            setTimeout(startConnection, 1000);
        });
    }

    connection.onclose(function (err) {
        if (err) {
            console.error(err.toString());
        }
        setTimeout(startConnection, 1000);
    });

    startConnection();
}

