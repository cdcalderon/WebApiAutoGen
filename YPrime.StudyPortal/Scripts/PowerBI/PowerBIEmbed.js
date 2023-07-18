window.onload = function () {
    LoadReportPageContent();
}

function LoadReportPageContent() {
    $.ajax({
        type: "GET",
        url: 'Analytics/GetAnalyticsInWorkspace',
        data: {},
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (json) {
            BuildUI(json);
            const vm = new TabPage();
            vm.init('0');
            if (json != null) {
                StartEmbed(json[0], 0);
            }
        },
        error: function (e) {
            console.log("Error in LoadReportPageContent(): " + e.responseText);
        }
    });
}

function BuildUI(ReportsJson) {
    for (let reportIndex = 0; reportIndex < ReportsJson.length; reportIndex++) {
        AddTabReportHTML(reportIndex);
        CreateTab(ReportsJson[reportIndex], reportIndex);
    }

    function AddTabReportHTML(i) {
        const html = document.createElement("div");

        html.className = "bhoechie-tab-content active";
        html.innerHTML = `<div class="col-lg-12">
                                    <div class="form-horizontal">
                                        <div class="embeded-report" id="embedDiv` + i + `">
                                        </div>
                                    </div>
                                </div>`;

        const target = document.getElementById('TabReportContainer');

        target.appendChild(html);
    }

    function CreateTab(externalReport, tabIndex) {
        const tab = document.createElement("a");
        tab.setAttribute("tabpage", tabIndex.toString());
        tab.setAttribute("href", "#");
        //If this is the first element in the list, set it as active otherwise, hide it
        if (tabIndex === 1) {
            tab.setAttribute("class", "list-group-item active text-center");
        } else {
            tab.setAttribute("class", "list-group-item text-center");
        }

        tab.innerHTML = externalReport.Report.Name;

        tab.onclick = ScopePreserver(externalReport, tabIndex);

        const listGroup = document.getElementById("listGroup");
        listGroup.appendChild(tab);
    }
}

function StartEmbed(externalReport, i) {
    $.ajax({
        type: "GET",
        url: 'Analytics/GetEmbedConfig',
        data: { "analyticsIdString": externalReport.Report.Id },
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        async: "true",
        success: function (EmbedConfigJson) {
            if (EmbedConfigJson.ErrorMessage == null) {
                const config = GetConfig(EmbedConfigJson, externalReport.IsSponsorReport);
                EmbedReport(config, 'embedDiv' + i);
            } else {
                console.log('StartEmbed error: ' + EmbedConfigJson.ErrorMessage);
            }
        },
        error: function (e) {
            console.log('Failure getting EmbedConfig for report ' + externalReport.Report.Name + ', error: ' + e.responseText);
        }
    });
}

function ScopePreserver(externalReport, i) {
    return function () {
        StartEmbed(externalReport, i);
    };
}

function EmbedReport(config, element) {
    // Get a reference to the embedded report HTML element
    const embedContainer = document.getElementById(element);

    // Embed the report and display it within the div container.
    const report = powerbi.embed(embedContainer, config);

    report.on("error", function (event) {
        console.log(event.detail);

        report.off("error");
    });
}

function GetConfig(json, isSponsorReport) {
    const models = window['powerbi-client'].models;
    const permissions = models.Permissions.View;

    let filter = [];

    if (isSponsorReport) {
        filter = [{
            $schema: "https://powerbi.com/product/schema#basic",
            target: {
                table: "YPAuthSponsorStudy",
                column: "SponsorGuid"
            },
            operator: "In",
            values: [json.SponsorID]
        }];
    } else {
        filter = [{
            $schema: "https://powerbi.com/product/schema#basic",
            target: {
                table: "Study",
                column: "StudyID"
            },
            operator: "In",
            values: [json.StudyID]
        },
        {
            $schema: "https://powerbi.com/product/schema#basic",
            target: { // as long as the Site table is used within the report's model, this filter will remove sites not assigned to the user
                table: "Site",
                column: "SiteId"
            },
            operator: "In", // checks if Site.SiteId is IN list of user sites provided below through serialized EmbedConfig.cs
            values: json.UserSites
        }];
    }

    return {
        type: 'report',
        tokenType: 1,
        accessToken: json.EmbedToken.Token,
        embedUrl: json.EmbedUrl,
        id: json.ReportId,
        permissions: permissions,
        filters: filter,
        settings: {
            panes: {
                filters: {
                    visible: false
                },
                pageNavigation: {
                    visible: true
                }
            }
        }
    };
}
