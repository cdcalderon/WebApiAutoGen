function SessionTimeoutVm(sessionTimeout, sessionWarningTime, refreshAPI, loginUrl, baseUrl) {
    var self = this;
    self.SessionTimeout = sessionTimeout * 60; // Minutes * seconds.
    self.SessionWarningTime = sessionWarningTime * 60; // Minutes * seconds.
    self.LoginUrl = loginUrl;
    self.BasePath = baseUrl;

    self.RefreshAPI = refreshAPI;
    self.IsModalShown = false;
    self.Worker = null;
    self.TimeLostFocus = null;

    self.init = function () {
        self.setWorkerPath()
        self.startSessionTimeout();
        self.bindEvents();
    };

    self.setWorkerPath = function () {
        if (!self.BasePath.endsWith("/")) {
            self.BasePath += "/";
        }

        self.WorkerPath = self.BasePath + "scripts/timeout-worker.js";
    }

    self.startSessionTimeout = function () {
        self.Worker = new Worker(self.WorkerPath);

        var timeoutMs = self.SessionTimeout * 1000;
        self.Worker.postMessage(["startTimer", timeoutMs]);
    };

    self.executeTimerStep = function (remainingSeconds, shouldPing) {
        if (remainingSeconds <= self.SessionWarningTime) {
            self.showModal();
        }

        if (self.IsModalShown) {
            self.logTimeLeft("sessionCountdown", remainingSeconds);
        }

        if (remainingSeconds < 0) {
            self.logout();
        }
    };

    self.refreshSession = function () {
        if (self.Worker && !self.IsModalShown) {
           self.Worker.postMessage(["resetTimer"]);
        }
    };

    self.showModal = function () {
        $("#sessionTimeout").modal({
            keyboard: false,
            backdrop: "static"
        }).show();

        self.IsModalShown = true;
    };

    self.bindEvents = function () {
        self.Worker.onmessage = function (e) {
            var messageType = e.data.messageType;

            switch (messageType) {
                case "executeTimerStep":
                    var remainingSeconds = e.data.remainingSeconds;
                    var shouldPing = e.data.shouldPing;
                    self.executeTimerStep(remainingSeconds, shouldPing);
                    break;
                default:
                    break;

            }
        };
        $("#expireSession").click(function(e) {
            self.logout();
            e.preventDefault();
        });
        $("#refreshSession").click(function (e) {
            self.IsModalShown = false;
            self.refreshSession();
            e.preventDefault();
        });
        $(document).mousemove(function () {
            self.refreshSession();
        });
        $(document).keydown(function () {
            self.refreshSession();
        });
        window.addEventListener("blur", self.tabLostFocus);
        window.addEventListener("focus", self.tabGainedFocus);
    };

    self.tabLostFocus = function () {
        self.TimeLostFocus = Date.now();
    }

    self.tabGainedFocus = function () {
        if (!self.TimeLostFocus) {
            return;
        }

        var timeAway = Date.now() - self.TimeLostFocus;

        if (timeAway / 1000 > self.SessionTimeout) {
            self.logout();
        } else {
            self.TimeLostFocus = null;
        }
    }

    self.logout = function() {
        $("#btnLogout")[0].click();
    };

    self.redirectToLogin = function() {
        window.location.href = self.LoginUrl;
    };

    self.logTimeLeft = function(elementId, timeLeft) {

        if (timeLeft < 0) {
            minutes = "00";
            seconds = "00";
        } else {
            minutes = parseInt(timeLeft / 60, 10);
            seconds = parseInt(timeLeft % 60, 10);

            minutes = minutes < 10 ? "0" + minutes : minutes;
            seconds = seconds < 10 ? "0" + seconds : seconds;
        }

        if (minutes !== "00") {
            $("#" + elementId).text(minutes + ":" + seconds);
        } else if (timeLeft < 0) {
            $("#" + elementId).text(0);
        } else {
            $("#" + elementId).text(seconds);
        }
    };
}