var colorRange = d3.scale.category20().range();
var dollarFormat = "$1,";
var tickOffset = "8px";
var d3TickSize = 1;

var initSvg = function(element,
    startWidth,
    startHeight,
    topOffset,
    rightOffset,
    bottomOffset,
    leftOffset,
    legendLeftOffset,
    isPieChart,
    includeLegend) {

    var radius = ((startWidth > startHeight) ? startHeight : startWidth) * 0.8 / 2;

    var dimensions = {
        width: startWidth,
        height: startHeight,
        top: topOffset,
        right: rightOffset,
        bottom: bottomOffset,
        left: leftOffset,
        innerWidth: startWidth - (rightOffset + leftOffset), // legendLeftOffset - rightOffset,//
        innerHeight: startHeight - (topOffset + bottomOffset),
        radius: radius //(startWidth - (rightOffset + leftOffset)) * 0.6 / 2 // 0.6 is the width to height aspect ratio
    };

    var svg = d3.select(element).append("svg")
        //responsive SVG needs these 2 attributes and no width and height attr
        .attr("preserveAspectRatio", "xMinYMax meet")
        .attr("viewBox", "0 0 " + startWidth + " " + startHeight)
        .classed("svg-content-responsive", true);

    var chartStartLeft = dimensions.left;
    var chartStartTop = dimensions.top;
    if (isPieChart) {
        chartStartTop = (startHeight / 2);
        if (includeLegend) {
            chartStartLeft = (startWidth / 5) + 20; // dimensions.left + (dimensions.innerWidth / 2);    
            legendLeftOffset = startWidth / 2;
        } else {
            chartStartLeft = (startWidth / 2); // dimensions.left + (dimensions.innerWidth / 2);    
        }
    }
    var chart = svg.append("g")
        .attr("transform", "translate(" + chartStartLeft + "," + chartStartTop + ")");

    var legend = svg.append("g")
        .attr("transform", "translate(" + legendLeftOffset + "," + topOffset + ")");

    var baseText = svg
        .append("text")
        .style("text-anchor", "middle")
        .style("font-weight", "bold")
        .attr("dx", isPieChart ? (dimensions.innerWidth / 2) : (dimensions.left + (dimensions.innerWidth / 2)))
        .attr("dy", isPieChart ? dimensions.innerHeight - 50 : dimensions.height - 5);

    return {
        chart: chart,
        legend: legend,
        container: svg,
        dimensions: dimensions,
        baseText: baseText
    };
};

var calculateHeight = function(container, chartConfig) {
    var height = 355;
    var mainOffset = chartConfig.ShowDisplayTitle ? 107 : 67;

    if ($(container).height() > 0) {
        height = $(container).height();
    }

    if ($(container).closest(".widget").length > 0) {
        //check for h3 title
        var titleOffset = 0;
        var h3 = $(container).closest(".widget").find("h3");
        if (h3.length > 0) {
            titleOffset = $(h3).height();
        }

        height = $(container).closest(".widget").innerHeight() - titleOffset - mainOffset;

    }

    return height;
};

var loadChart = function(id, chartConfig) {
    var svg;
    var chart;
    var legend;
    var dimensions;
    var color = d3.scale.ordinal()
        .range(colorRange);
    var chartDataString = $("#chartData-" + id).val();
    var chartDataObject = JSON.parse(chartDataString);
    var chartType = chartDataObject.CustomChartTypes == undefined
        ? chartDataObject.ChartType
        : chartDataObject.CustomChartTypes[0];

    var container = $("#" + id);
    var minWidth = chartConfig == undefined || chartConfig.minWidth == undefined ? 650 : chartConfig.minWidth;
    var containerWidth = $(container).width() > 0 ? $(container).width() : minWidth;
    var containerHeight = //$(container).height() > 0  ? $(container).height() : 355;
        calculateHeight(container, chartDataObject);
    var legendLeftOffset = calculateLegendLeftOffset(containerWidth); //440;
    //offsets
    var topOffset = chartConfig == undefined || chartConfig.TopOffset == undefined ? 9 : chartConfig.TopOffset;

    var defaultLeft = 60;
    if (chartType == 35) {
        defaultLeft = 0;
        containerHeight = 150;
    }

    //vso #25042 - make sure legend is not over data - jo 27Nov2017
    var rightOffset = chartDataObject.IncludeLegend ? containerWidth - legendLeftOffset : 0;
    var bottomOffset = 100;
    var leftOffset = defaultLeft;

    function calculateLegendLeftOffset(width) {
        return (width * 1) * .75;
    }

    var isPieChart = (chartType == 17 || chartType == 18) ? true : false;

    svg = initSvg("#chartContainer-" + id,
        containerWidth,
        containerHeight,
        topOffset,
        rightOffset,
        bottomOffset,
        leftOffset,
        legendLeftOffset,
        isPieChart,
        chartDataObject.IncludeLegend);

    chart = svg.chart;
    legend = svg.legend;
    dimensions = svg.dimensions;

    var xLabels = $.map(chartDataObject.XLabels,
        function(value, index) {
            return [value];
        });

    var yLabels = $.map(chartDataObject.YLabels,
        function(value, index) {
            return [value];
        });

    var yScale = d3.scale.linear()
        .domain([
            0, d3.max(chartDataObject.ChartSeries,
                function(d) {
                    return d3.max(d.SeriesDataPoints,
                        function(dd) {
                            return dd.Y;
                        });
                })
        ])
        .range([dimensions.innerHeight, 0]);
    var seriesNames = [];
    for (var count = 0; count < chartDataObject.ChartSeries.length; count++) {
        var series = chartDataObject.ChartSeries[count];
        seriesNames[count] = chartDataObject.ChartSeries[count].SeriesName;
        for (i = 0; i < series.SeriesDataPoints.length; i++) {
            //   series.SeriesDataPoints[i].Color = series.SeriesColor ? series.SeriesColor : colorRange[count];
            series.SeriesDataPoints[i].SeriesName = series.SeriesName;
            series.SeriesDataPoints[i].SeriesStyle = series.SeriesStyle;
        }
    }
    draw(chartDataObject, chartConfig);

    function getPieChartLegendData() {
        var names = [];
        for (i = 0; i < chartDataObject.ChartSeries[0].SeriesDataPoints.length; i++) {
            names[i] = chartDataObject.ChartSeries[0].SeriesDataPoints[i].YLabel;
        }

        return names;
    }

    function draw(data, chartConfig) {

        switch (chartType) {
        case 3: // Line  
            drawLine();
            break;
        case 7: // Bar
            drawBar(chartConfig);
            break;
        case 17: // Pie 
            drawPie();
            break;
        case 18: // Donut 
            drawDonut();
            break;
        case 35: // Fill Gauge 
            drawFillGauge();
            break;
        }

        if (data.IncludeLegend) {
            drawLegend();
        }

        function drawLegend() {
            var legendData = isPieChart ? getPieChartLegendData() : seriesNames.slice();

            var legendItems = legend
                .selectAll("g")
                .data(legendData)
                .enter().append("g")
                .attr("transform", function(d, i) { return "translate(10," + i * 20 + ")"; });

            legendItems.append("rect")
                .attr("width", 18)
                .attr("height", 18)
                .style("fill",
                    function(d, i) {
                        return color(d);
                    });

            legendItems.append("text")
                .attr("x", 24)
                .attr("y", 9)
                .attr("dy", ".25em")
                .attr("class", "legend")
                .text(function(d) { return d; });

        }

        function drawBar(chartConfig) {

            var xScale = d3.scale.ordinal()
                .domain(xLabels)
                .rangeBands([0, dimensions.innerWidth], .1);

            var x1Scale = d3.scale.ordinal()
                .domain(d3.range(chartDataObject.ChartSeries.length))
                .rangeBands([0, xScale.rangeBand()]);

            var xAxis = d3.svg.axis()
                .scale(xScale)
                .tickSize(d3TickSize)
                .tickValues(xLabels)
                .orient("bottom");

            var yAxis = d3.svg.axis()
                .scale(yScale)
                .tickSize(d3TickSize)
                .orient("left");
            chart.append("g")
                .attr("class", "x axis")
                .attr("transform", "translate(0," + (dimensions.innerHeight) + ")")
                .call(xAxis)
                .selectAll("text")
                .style("text-anchor", "end")
                .attr("dx", "-1em")
                .attr("dy", tickOffset)
                .attr("transform", "rotate(-45)")
                .append("title")
                .text(function(d) { return d; });
            chart.append("g")
                .attr("class", "y axis")
                .call(yAxis)
                .selectAll("text")
                .attr("dx", "-" + tickOffset)
                .style("text-anchor", "end");

            tryFixChartHeight();

            var series = chart.selectAll("g.series")
                .data(data.ChartSeries)
                .enter()
                .append("svg:g")
                .attr("class", "series") // Not strictly necessary, but helpful when inspecting the DOM
                .attr("fill",
                    function(d, i) {
                        return color(d);
                    })
                .attr("transform",
                    function(d, i) {
                        return "translate(" + x1Scale(i) + ")";
                    });

            var chartData = data.ChartSeries[0].SeriesDataPoints;
            xScale.domain(chartData.map(function(d) { return d.X; }));
            yScale.domain([
                0, d3.max(data.ChartSeries,
                    function(d) {
                        return d3.max(d.SeriesDataPoints,
                            function(dd) {
                                return dd.Y;
                            });
                    })
            ]);
            var bar = series.selectAll("bar")
                .data(function(d) { return d.SeriesDataPoints });

            var rect = bar.enter()
                .append("rect")
                .style("fill",
                    function(d) {
                        return d.SeriesStyle != null
                            ? d.SeriesStyle.Fill
                            ? d.SeriesStyle.Fill
                            : color(d.SeriesName)
                            : color(d.SeriesName);
                    })
                .attr("x",
                    function(d) {
                        var xPos = xScale(d.X);
                        return xPos;
                    })
                .attr("width", x1Scale.rangeBand())
                .attr("y", yScale(0))
                .attr("height",
                    function(d) {
                        var yPos = yScale(d.Y);
                        return yPos;
                    })
                .on("mouseover",
                    function(d, index) {
                        $("#chartTooltip-" + id)
                            .html(d.SeriesName + "<br>" + d.X.toString() + (index != d.Y)
                                ? "<br>" + d.Y.toString()
                                : "")
                            .addClass("active")
                            .show();

                        d3.select(this).style("fill", chartDataObject.ChartSeriesMouseOverColor);

                        if (typeof chartConfig != "undefined" && chartConfig.mouseover != undefined) {
                            chartConfig.mouseover({ ChartDataPoint: d });
                        }
                    })
                .on("mousemove",
                    function(d) {
                        var offset = 15;
                        $("#chartTooltip-" + id)
                            .css("left", d3.event.layerX + offset)
                            .css("top", d3.event.layerY + offset);
                    })
                .on("mouseout",
                    function(d) {
                        $("#chartTooltip-" + id).html("").removeClass("active").hide();
                        d3.select(this).style("fill",
                            d.SeriesStyle != null
                            ? d.SeriesStyle.Fill
                            ? d.SeriesStyle.Fill
                            : color(d.SeriesName)
                            : color(d.SeriesName));

                        if (typeof chartConfig != "undefined" && chartConfig.mouseout != undefined) {
                            chartConfig.mouseout({ ChartDataPoint: d });
                        }
                    });

            rect.transition()
                .duration(300)
                .attr("y", function(d) { return yScale(d.Y); })
                .attr("height",
                    function(d) {
                        var barHeight = dimensions.innerHeight - yScale(d.Y);
                        return barHeight;
                    });
        }

        function tryFixChartHeight() {
            try {
                var svg = document.querySelector("svg g");
                if (svg == null)
                    return;

                var padding = 10;
                var svgHeight = svg.getBBox().height;
                var viewBox = document.querySelector("svg").getAttribute("viewBox").split(" ");
                viewBox[3] = svgHeight + padding;
                document.querySelector("svg").setAttribute("viewBox", viewBox.join(" "));
            } catch (e) {
                // we don't care about the errors for now
            }
        }

        function drawLine() {
            //vso#24350 - x label text is getting cut off - jo 27Oct2017
            var maxLabelLength = 0;
            for (var i = 0; i < xLabels.length; i++) {
                if (xLabels[i].length > maxLabelLength) {
                    maxLabelLength = xLabels[i].length;
                }
            }
            //css does not work on <text> elements
            var labelStyle = maxLabelLength > 20 ? "0.75em" : maxLabelLength > 10 ? "0.85em" : "1em";

            var xScaleNoBands = d3.scale.ordinal()
                .domain(xLabels)
                .rangePoints([0, dimensions.innerWidth]);

            var xAxis = d3.svg.axis()
                .scale(xScaleNoBands)
                .tickValues(xLabels)
                .tickSize(d3TickSize)
                .orient("bottom");

            var yAxis = d3.svg.axis()
                .scale(yScale)
                .tickSize(d3TickSize)
                .orient("left");
            var lineCreator = d3.svg.line()
                .x(function(d) {
                    return xScaleNoBands(d.X);
                })
                .y(function(d) {
                    return yScale(d.Y);
                })
                .interpolate("linear");
            chart.append("g")
                .attr("class", "x axis")
                .attr("transform", "translate(0," + (dimensions.innerHeight) + ")")
                .call(xAxis)
                .selectAll("text")
                .style({ "font-size": labelStyle, "text-anchor": "end" })
                .attr("dx", "-1em")
                .attr("dy", tickOffset)
                .attr("transform", "rotate(-45)");
            chart.append("g")
                .attr("class", "y axis")
                .call(yAxis)
                .selectAll("text")
                .attr("dx", "-" + tickOffset)
                .style("text-anchor", "end");

            //var chartData = data.ChartSeries[0].SeriesDataPoints;
            //xScaleNoBands.domain(chartData.map(function (d) {
            //    return d.X;
            //}));

            var xValues = [];
            var xValuesObject = {};
            for (var i = 0; i < data.ChartSeries.length; i++) {
                for (var j = 0; j < data.ChartSeries[i].SeriesDataPoints.length; j++) {
                    var dataPoint = data.ChartSeries[i].SeriesDataPoints[j];
                    if (!xValuesObject[dataPoint.X]) {
                        xValuesObject[dataPoint.X] = 1;
                    }
                }
            }
            for (var prop in xValuesObject) {
                xValues.push(prop);
            }

            xScaleNoBands.domain(xValues);
            data.ChartSeries.forEach(function(d) {
                var path = chart.append("path")
                    .attr("class", "line")
                    .attr("stroke", d.SeriesStyle.Fill ? d.SeriesStyle.Fill : color(d.SeriesName));
                if (d.SeriesStyle.Strokedash) {
                    path = path.attr("stroke-dasharray", d.SeriesStyle.Strokedash ? "4,4" : "0,0");
                }

                path.attr("d", lineCreator(d.SeriesDataPoints));

                if (!d.HidePlotPoints) { // Hide Plot points
                    chart.selectAll("dot")
                        .data(d.SeriesDataPoints)
                        .enter().append("circle")
                        .attr("r", 2.5)
                        .attr("cx", function(d) { return xScaleNoBands(d.X); })
                        .attr("cy", function(d) { return yScale(d.Y); })
                        .style("fill", "black")
                        .on("mouseover",
                            function(d) {
                                $("#chartTooltip-" + id)
                                    .html(d.SeriesName + "<br>" + d.X.toString() + "<br>" + d.Y.toString())
                                    .show();
                            })
                        .on("mousemove",
                            function(d) {
                                $("#chartTooltip-" + id)
                                    .css("left", d3.event.clientX)
                                    .css("top", d3.event.clientY);
                            })
                        .on("mouseout",
                            function(d) {
                                $("#chartTooltip-" + id).html("").hide();
                            });
                }
            });
        }

        function drawPie() {
            drawDonut(0);
        }

        function drawDonut(innerRadiusFactor) {
            var chart_radius = dimensions.radius;

            if (innerRadiusFactor == undefined) {
                innerRadiusFactor = 0.5;
            }

            var chartData = data.ChartSeries[0].SeriesDataPoints;
            var sum = d3.sum(chartData,
                function(d) {
                    return d.Y;
                });

            var pathAnim = function(path, dir) {
                switch (dir) {
                case 0:
                    path.transition()
                        .duration(500)
                        .ease("bounce")
                        .attr("d",
                            d3.svg.arc()
                            .innerRadius(chart_radius * innerRadiusFactor)
                            .outerRadius(chart_radius * .75)
                        );
                    break;

                case 1:
                    path.transition()
                        .attr("d",
                            d3.svg.arc()
                            .innerRadius(chart_radius * innerRadiusFactor)
                            .outerRadius(chart_radius * 0.85)
                        );
                    break;
                }
            };
            var arc = d3.svg.arc()
                .innerRadius(chart_radius * innerRadiusFactor)
                .outerRadius(chart_radius * .75);


            var pie = d3.layout.pie()
                .sort(null)
                .value(function(d) {
                    return d.Y;
                });


            var path = chart.selectAll(".arc")
                .data(pie(chartData))
                .enter().append("g")
                .attr("class", "arc");

            path.append("path")
                .attr("d", arc)
                .attr("id",
                    function (d) {
                        return getSanitizedPathId('donut', data.Id, d.data.XLabel);
                    })
                .style("fill",
                    function(d) {
                        return color(d.data.X);
                    })
                .on("mouseover",
                    function(d) {
                        pathAnim(d3.select(this), 1);
                        $("#centreLabel-" + id)
                            .text(d.data.XLabel + " - " + d.data.Y);

                    })
                .on("mouseout",
                    function(d) {
                        pathAnim(d3.select(this), 0);
                        $("#centreLabel-" + id)
                            .text("Total - " + sum);

                    });


            var centreLabel = chart.append("text")
                .attr("id", "centreLabel-" + id)
                .text("Total - " + sum)
                .attr("y", chart_radius + 10)
                .style("text-anchor", "middle")
                .style("font-weight", "bold")
                .style("font-size", "22");


        }

        function drawFillGauge() {
            // There can only be one Chart Series  and one data point for Fill Gauges
            var chartData = data.ChartSeries[0].SeriesDataPoints[0];

            loadLiquidFillGauge(chartData.Y);
        }
    };

    function getSanitizedPathId(
        chartTypeName,
        chartName,
        sectionName)
    {
        var concatenatedId = chartTypeName + '_' + chartName + '_' + sectionName;
        concatenatedId = concatenatedId.replaceAll(' ', '').toLowerCase();

        return concatenatedId;
    }

    function liquidFillGaugeDefaultSettings() {
        return {
            minValue: 0, // The gauge minimum value.
            maxValue: 100, // The gauge maximum value.
            circleThickness: 0.05, // The outer circle thickness as a percentage of it's radius.
            circleFillGap:
                0.05, // The size of the gap between the outer circle and wave circle as a percentage of the outer circles radius.
            circleColor: "#178BCA", // The color of the outer circle.
            waveHeight: 0.10, // The wave height as a percentage of the radius of the wave circle.
            waveCount: 1, // The number of full waves per width of the wave circle.
            waveRiseTime: 2000, // The amount of time in milliseconds for the wave to rise from 0 to it's final height.
            waveAnimateTime: 2000, // The amount of time in milliseconds for a full wave to enter the wave circle.
            waveRise: true, // Control if the wave should rise from 0 to it's full height, or start at it's full height.
            waveHeightScaling:
                true, // Controls wave size scaling at low and high fill percentages. When true, wave height reaches it's maximum at 50% fill, and minimum at 0% and 100% fill. This helps to prevent the wave from making the wave circle from appear totally full or empty when near it's minimum or maximum fill.
            waveAnimate: true, // Controls if the wave scrolls or is static.
            waveColor: "#178BCA", // The color of the fill wave.
            waveOffset: 0, // The amount to initially offset the wave. 0 = no offset. 1 = offset of one full wave.
            textVertPosition:
                .5, // The height at which to display the percentage text withing the wave circle. 0 = bottom, 1 = top.
            textSize: 0.8, // The relative height of the text to display in the wave circle. 1 = 50%
            valueCountUp:
                true, // If true, the displayed value counts up from 0 to it's final value upon loading. If false, the final value is displayed.
            displayPercent: true, // If true, a % symbol is displayed after the value.
            textColor: "#045681", // The color of the value text when the wave does not overlap it.
            waveTextColor: "#A4DBf8" // The color of the value text when the wave overlaps it.
        };
    }

    function loadLiquidFillGauge(value, config) {
        if (config == null) config = liquidFillGaugeDefaultSettings();

        var radius = dimensions.radius; // TO DO: Outer Radius for all charts is actualy 0.85 . Need to fix this later
        var locationX = parseInt(dimensions.innerWidth) / 2 - radius;

        var locationY = 0;
        var fillPercent = Math.max(config.minValue, Math.min(config.maxValue, value)) / config.maxValue;

        var waveHeightScale;
        if (config.waveHeightScaling) {
            waveHeightScale = d3.scale.linear()
                .range([0, config.waveHeight, 0])
                .domain([0, 50, 100]);
        } else {
            waveHeightScale = d3.scale.linear()
                .range([config.waveHeight, config.waveHeight])
                .domain([0, 100]);
        }

        var textPixels = (config.textSize * radius / 2);
        var textFinalValue = parseFloat(value).toFixed(2);
        var textStartValue = config.valueCountUp ? config.minValue : textFinalValue;
        var percentText = config.displayPercent ? "%" : "";
        var circleThickness = config.circleThickness * radius;
        var circleFillGap = config.circleFillGap * radius;
        var fillCircleMargin = circleThickness + circleFillGap;
        var fillCircleRadius = radius - fillCircleMargin;
        var waveHeight = fillCircleRadius * waveHeightScale(fillPercent * 100);

        var waveLength = fillCircleRadius * 2 / config.waveCount;
        var waveClipCount = 1 + config.waveCount;
        var waveClipWidth = waveLength * waveClipCount;

        // Rounding functions so that the correct number of decimal places is always displayed as the value counts up.
        var textRounder = function(value) { return Math.round(value); };
        if (parseFloat(textFinalValue) != parseFloat(textRounder(textFinalValue))) {
            textRounder = function(value) { return parseFloat(value).toFixed(1); };
        }
        if (parseFloat(textFinalValue) != parseFloat(textRounder(textFinalValue))) {
            textRounder = function(value) { return parseFloat(value).toFixed(2); };
        }

        // Data for building the clip wave area.
        var data = [];
        for (var i = 0; i <= 40 * waveClipCount; i++) {
            data.push({ x: i / (40 * waveClipCount), y: (i / (40)) });
        }

        // Scales for drawing the outer circle.
        var gaugeCircleX = d3.scale.linear().range([0, 2 * Math.PI]).domain([0, 1]);
        var gaugeCircleY = d3.scale.linear().range([0, radius]).domain([0, radius]);

        // Scales for controlling the size of the clipping path.
        var waveScaleX = d3.scale.linear().range([0, waveClipWidth]).domain([0, 1]);
        var waveScaleY = d3.scale.linear().range([0, waveHeight]).domain([0, 1]);

        // Scales for controlling the position of the clipping path.
        var waveRiseScale = d3.scale.linear()
            // The clipping area size is the height of the fill circle + the wave height, so we position the clip wave
            // such that the it will overlap the fill circle at all when at 0%, and will totally cover the fill
            // circle at 100%.
            .range([(fillCircleMargin + fillCircleRadius * 2 + waveHeight), (fillCircleMargin - waveHeight)])
            .domain([0, 1]);
        var waveAnimateScale = d3.scale.linear()
            .range([0, waveClipWidth - fillCircleRadius * 2]) // Push the clip area one full wave then snap back.
            .domain([0, 1]);

        // Scale for controlling the position of the text within the gauge.
        var textRiseScaleY = d3.scale.linear()
            .range([fillCircleMargin + fillCircleRadius * 2, (fillCircleMargin + textPixels * 0.7)])
            .domain([0, 1]);

        // Center the gauge within the parent SVG.
        var gaugeGroup = chart.append("g")
            .attr("transform", "translate(" + locationX + "," + locationY + ")");

        // Draw the outer circle.
        var gaugeCircleArc = d3.svg.arc()
            .startAngle(gaugeCircleX(0))
            .endAngle(gaugeCircleX(1))
            .outerRadius(gaugeCircleY(radius))
            .innerRadius(gaugeCircleY(radius - circleThickness));
        gaugeGroup.append("path")
            .attr("d", gaugeCircleArc)
            .style("fill", config.circleColor)
            .attr("transform", "translate(" + radius + "," + radius + ")");

        // Text where the wave does not overlap.
        var text1 = gaugeGroup.append("text")
            .text(textRounder(textStartValue) + percentText)
            .attr("class", "liquidFillGaugeText")
            .attr("text-anchor", "middle")
            .attr("font-size", textPixels + "px")
            .style("fill", config.textColor)
            .attr("transform", "translate(" + radius + "," + textRiseScaleY(config.textVertPosition) + ")");

        // The clipping wave area.
        var clipArea = d3.svg.area()
            .x(function(d) { return waveScaleX(d.x); })
            .y0(function(d) {
                return waveScaleY(Math.sin(Math.PI * 2 * config.waveOffset * -1 +
                    Math.PI * 2 * (1 - config.waveCount) +
                    d.y * 2 * Math.PI));
            })
            .y1(function(d) { return (fillCircleRadius * 2 + waveHeight); });
        var waveGroup = gaugeGroup.append("defs")
            .append("clipPath")
            .attr("id", "clipWave" + id);
        var wave = waveGroup.append("path")
            .datum(data)
            .attr("d", clipArea)
            .attr("T", 0);

        // The inner circle with the clipping wave attached.
        var fillCircleGroup = gaugeGroup.append("g")
            .attr("clip-path", "url(#clipWave" + id + ")");
        fillCircleGroup.append("circle")
            .attr("cx", radius)
            .attr("cy", radius)
            .attr("r", fillCircleRadius)
            .style("fill", config.waveColor);

        // Text where the wave does overlap.
        var text2 = fillCircleGroup.append("text")
            .text(textRounder(textStartValue) + percentText)
            .attr("class", "liquidFillGaugeText")
            .attr("text-anchor", "middle")
            .attr("font-size", textPixels + "px")
            .style("fill", config.waveTextColor)
            .attr("transform", "translate(" + radius + "," + textRiseScaleY(config.textVertPosition) + ")");

        // Make the value count up.
        if (config.valueCountUp) {
            var textTween = function() {
                var i = d3.interpolate(this.textContent, textFinalValue);
                return function(t) { this.textContent = textRounder(i(t)) + percentText; };
            };
            text1.transition()
                .duration(config.waveRiseTime)
                .tween("text", textTween);
            text2.transition()
                .duration(config.waveRiseTime)
                .tween("text", textTween);
        }

        // Make the wave rise. wave and waveGroup are separate so that horizontal and vertical movement can be controlled independently.
        var waveGroupXPosition = fillCircleMargin + fillCircleRadius * 2 - waveClipWidth;
        if (config.waveRise) {
            waveGroup.attr("transform", "translate(" + waveGroupXPosition + "," + waveRiseScale(0) + ")")
                .transition()
                .duration(config.waveRiseTime)
                .attr("transform", "translate(" + waveGroupXPosition + "," + waveRiseScale(fillPercent) + ")")
                .each("start",
                    function() {
                        wave.attr("transform", "translate(1,0)");
                    }); // This transform is necessary to get the clip wave positioned correctly when waveRise=true and waveAnimate=false. The wave will not position correctly without this, but it's not clear why this is actually necessary.
        } else {
            waveGroup.attr("transform", "translate(" + waveGroupXPosition + "," + waveRiseScale(fillPercent) + ")");
        }


        if (config.waveAnimate) animateWave(config);

        function animateWave(c) {
            wave.attr("transform", "translate(" + waveAnimateScale(wave.attr("T")) + ",0)");
            wave.transition()
                .duration(c.waveAnimateTime * (1 - wave.attr("T")))
                .ease("linear")
                .attr("transform", "translate(" + waveAnimateScale(1) + ",0)")
                .attr("T", 1)
                .each("end",
                    function() {
                        wave.attr("T", 0);
                        animateWave(c);
                    });
        }


    }
};


function GetChartHtml(id) {
    function resize() {
        var widthOffset = 150;
        var heightOffset = 150;
        $(".modal-dialog").width($(window).width() - widthOffset);
        $(".modal-dialog").height($(window).height() - heightOffset);
    }

    setTimeout(resize, 1000);
    return $("#" + id).html();
}