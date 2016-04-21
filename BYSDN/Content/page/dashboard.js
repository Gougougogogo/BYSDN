$(document).ready(function () {
    setSummaryData();
});

function setSummaryData() {
    $.ajax({
        url: "../Home/GetSummaryData",
        method: 'Get',
        dataType: 'json',
        cache: false,
        data: {},
        success: function (e) {
            if (e.success) {
                var dataSource = e.retData;
                $("#summary").dxChart({
                    dataSource: dataSource,
                    commonSeriesSettings: {
                        argumentField: "Day"
                    },
                    series: [
                        { valueField: "Question", name: "Question Count", color: "#40bbea" },
                        { valueField: "Answer", name: "Answer Count", color: "#cc3f44" },
                    ],
                    argumentAxis: {
                        grid: {
                            visible: true
                        }
                    },
                    tooltip: {
                        enabled: true,
                        argumentPrecision: 2,
                        precision: 2,
                        customizeTooltip: function (point) {
                            return {
                                text: 'Day (' + point.argumentText + ') of this month\'s ' + point.seriesName + ' is ' + point.valueText
                            };
                        }
                    },
                    title: "Question and Answers summary",
                    legend: {
                        verticalAlignment: "bottom",
                        horizontalAlignment: "center"
                    },
                    commonPaneSettings: {
                        border: {
                            visible: true,
                            right: false
                        }
                    },
                    animation: {
                        duration: 2000,
                        easing: 'linear'
                    }
                });
            }
            else {
                layer.msg(e.retData);
            }
        }
    });
}