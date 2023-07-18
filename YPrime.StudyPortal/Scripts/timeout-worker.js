// messages should be in the format:
// e.data[0] == messageType

// for startTimer message types:
// e.data[1] == timeout period is ms (ex: 900000 for 15 minutes)

// for resetTimer message types no extra data is expected

var timerIntervalMs = 1000;
var timeoutInterval = 1000;
var pingTimeThresholdMs = 30000;
var timeoutDate = Date.now();
var expectedCheckTime = Date.now();
var pingCheckTime = Date.now();

onmessage = function (e) {
    var messageType = e.data[0];

    switch (messageType) {
        case "startTimer":
            var timeoutPeriod = e.data[1];
            startTimer(timeoutPeriod);
            break;
        case "resetTimer":
            resetTimer();
        default:
            break;
    }
}

function startTimer(passedInTimeoutInterval) {

    timeoutInterval = passedInTimeoutInterval;
    resetTimer();

    expectedCheckTime = Date.now() + timerIntervalMs;
    resetPingTime();

    setTimeout(timerStep, timerIntervalMs);
}

function resetTimer() {
    timeoutDate = Date.now() + timeoutInterval;
}

function resetPingTime() {
    pingCheckTime = Date.now() + pingTimeThresholdMs;
}

function timerStep() {
    var driftTime = Date.now() - expectedCheckTime;
    var remainingTime = timeoutDate - Date.now();
    var remainingPingTime = pingCheckTime - Date.now();

    if (driftTime > timerIntervalMs) {
        // call is being throttled
        console.log("log out timer is being throttled");
    }

    var shouldPing = false;

    if (remainingPingTime <= 0) {
        shouldPing = true;
        resetPingTime();
    }

    postMessage({
        messageType: "executeTimerStep",
        remainingSeconds: remainingTime / 1000,
        shouldPing: shouldPing
    });

    expectedCheckTime += timerIntervalMs;

    setTimeout(timerStep, Math.max(0, timerIntervalMs - driftTime));
}