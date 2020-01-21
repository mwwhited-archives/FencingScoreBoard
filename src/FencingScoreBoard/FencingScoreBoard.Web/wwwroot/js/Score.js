var model = {
    "playerRightName": " ", //"1234567890123456789",
    "playerRightClub": " ", //"12345678901234567890123456789012",
    "playerRightScore": "0", //"99",
    "playerRightCard": "transparent", //"red",
    "playerRightLight": "transparent", //"red",

    "playerLeftName": " ", //"9999999999888888888",
    "playerLeftClub": " ", //"99999999998888888888777777777766",
    "playerLeftScore": "0", //"99",
    "playerLeftCard": "transparent", //"yellow",
    "playerLeftLight": "transparent", //"green",

    "clock": "0:00" //"3:00"
};

function copyTo(source, target) {
    for (var key in source) {
        target[key] = source[key];
    }
    return target;
}

function updateWith(source, target) {
    copyTo(source, target);
    updateData(target);
    return target;
}

function getColor(color) {
    if (color) {
        return color.toString();
    }
    return "transparent";
}

function updateData(source) {
    var textFields = ["playerRightName", "playerRightClub", "playerRightScore", "playerLeftName", "playerLeftClub", "playerLeftScore", "clock"];
    for (var idx1 in textFields) {
        var field1 = textFields[idx1];
        if (source[field1] && document.getElementById(field1)) {
            document.getElementById(field1).textContent = source[field1];
        }
    }

    var colorFields = ["playerRightCard", "playerRightLight", "playerLeftCard", "playerLeftLight"];
    for (var idx2 in colorFields) {
        var field2 = colorFields[idx2];
        if (source[field2] && document.getElementById(field2)) {
            document.getElementById(field2).style.backgroundColor = source[field2];
        }
    }

    /*
    var colorFields = ["playerRightPriority", "playerLeftPriority"];
    for (var idx3 in colorFields) {
        var field3 = colorFields[idx3];
        if (source[field3] && document.getElementById(field3)) {
            document.getElementById(field3).style.backgroundColor = source[field3] ? "yellow" : "green";
        }
    } 
    */

    var displayFields = ["content"];
    for (var idx4 in displayFields) {
        var field4 = displayFields[idx4];
        if (source[field4] && document.getElementById(field4)) {
            document.getElementById(field4).style.display = source[field4];
        }
    }
}

function setZoom() {
    var widthRatio = ((window.innerWidth > 0) ? window.innerWidth : screen.width) / 1920;
    var heightRatio = ((window.innerHeight > 0) ? window.innerHeight : screen.height) / 1080;
    console.log("widthRatio:" + widthRatio);
    console.log("heightRatio:" + heightRatio);
    var zoom = widthRatio > heightRatio ? heightRatio : widthRatio;
    document.documentElement.style.zoom = zoom;
}

function onReceiveData(message, connection) {
    model = updateData(message.data, model);
}

function onDocumentReady(connection) {
    setZoom();
}

window.ScoreMachineVM = window.ScoreMachineVM || {};
window.ScoreMachineVM.onReceiveData = onReceiveData;
window.ScoreMachineVM.onDocumentReady = onDocumentReady;