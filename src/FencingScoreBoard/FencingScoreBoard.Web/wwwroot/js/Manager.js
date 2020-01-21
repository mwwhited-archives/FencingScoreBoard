window.ScoreMachineHub = window.ScoreMachineHub || {};

function addMsg(message) {
    var node = document.createElement("LI");
    var textnode = document.createTextNode(JSON.stringify(message));
    node.appendChild(textnode);
    document.getElementById("received").appendChild(node);
}

function getInputs() {
    var model = {
        "messageType": "ScoreMachine"
    };
    var inputs = document.querySelectorAll('input,select');
    for (var key in inputs) {
        var inp = inputs[key];
        if (inp.name && inp.value) {
            model[inp.name] = inp.value;
        }
    }

    model.playerRightName = model.playerRightName || " ";
    model.playerRightClub = model.playerRightClub || " ";

    model.playerRightName = model.playerRightName || " ";
    model.playerLeftClub = model.playerLeftClub || " ";

    return model;
}

function updateInputs(data, recordingStatus) {
    if (data && data.messageType && data.messageType === "ScoreMachine") {
        console.log("score machine received");
        for (var idx in data) {
            console.log("score machine idx: " + idx);
            var elm = document.getElementById(idx);
            if (elm) {
                var value = data[idx];
                console.log("score machine value: " + value);
                elm.value = value;
            }
        }
    }
    console.log("recordingStatus: " + recordingStatus);
    document.getElementById("recording-status").innerText = recordingStatus ? "Recording" : "Stopped";
}

function onReceiveData(message) {
    addMsg(JSON.stringify(message));

    if (message.data) {
        updateInputs(message.data, message.recording);
    }
}

function onDocumentReady(connection) {
    document.getElementById("Post").addEventListener("click", function (event) {
        var model = getInputs();
        window.ScoreMachineHub.sendData(model);
        event.preventDefault();
    });
    document.getElementById("StartBout").addEventListener("click", function (event) {
        window.ScoreMachineHub.startBout();
        event.preventDefault();
    });
    document.getElementById("EndBout").addEventListener("click", function (event) {
        window.ScoreMachineHub.endBout();
        event.preventDefault();
    });
    document.getElementById("ClearReceived").addEventListener("click", function (event) {
        var receivedElm = document.getElementById("received");
        while (receivedElm.hasChildNodes()) {
            receivedElm.removeChild(receivedElm.lastChild);
        }
        event.preventDefault();
    });
    document.getElementById("SwapPlayers").addEventListener("click", function (event) {
        var model = getInputs();

        console.log('swap players:'.concat(model.playerRightName, " => ", model.playerLeftName));
        var tempplayerRight = {
            playerRightName: model.playerRightName,
            playerRightClub: model.playerRightClub
        };

        model.playerRightName = model.playerLeftName || " ";
        model.playerRightClub = model.playerLeftClub || " ";

        model.playerLeftName = tempplayerRight.playerRightName || " ";
        model.playerLeftClub = tempplayerRight.playerRightClub || " ";


        window.ScoreMachineHub.sendData(model);
        event.preventDefault();
    });
    document.getElementById("ChangeVideo").addEventListener("click", function (event) {
        console.log('ChangeVideo');
        var targetChannel = document.getElementById("channel").value;
        console.log('ChangeVideo:'.concat(targetChannel));
        window.ScoreMachineHub.setChannel(targetChannel);
        event.preventDefault();
    });
}

window.ScoreMachineVM = window.ScoreMachineVM || {};
window.ScoreMachineVM.onReceiveData = onReceiveData;
window.ScoreMachineVM.onDocumentReady = onDocumentReady;