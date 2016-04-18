function mouseUp(e) {
    var $input = $("#searchText"),
      oldValue = $input.val();

    if (oldValue == "" || oldValue == undefined) return;

    // When this event is fired after clicking on the clear button
    // the value is not cleared yet. We have to wait for it.
    setTimeout(function () {
        var newValue = $input.val();

        if (newValue == "") {
            // Gotcha
            getBBSContent(currentPage);
        }
    }, 1);
};
function getBBSContent(page) {
    $.ajax({
        url: "../BBS/RequestQuestionList",
        method: 'Get',
        dataType: 'json',
        cache: false,
        data: {
            page : page
        },
        success: function (e) {
            if (e.success) {
                var html = '';
                $.each(e.retData, function (index)
                {
                    html += '<li>\
                                <a href="/BBS/Details/' + e.retData[index].BBSId + '">\
                                    <div class="row">\
                                        <div class="col-xs-1">\
                                            <img style="margin-left:10px" src="' + e.retData[index].UserImage + '" width="30" />\
                                        </div>\
                                        <div class="col-xs-8">\
                                            <p class="bbsTitle">'+ e.retData[index].Title + '</p>\
                                        </div>\
                                        <div class="col-xs-3" style="text-align:right">\
                                            <p>'+ e.retData[index].PublishDate + '</p>\
                                        </div>\
                                    </div>\
                                    <p style="margin-left:60px">' + e.retData[index].Brief + '</p>\
                                </a>\
                            </li>';
                });
                $('#bbsList').html(html);
            }
            else {
                layer.msg(e.retData);
            }
        },
        error: function (e) {
            layer.msg(e.retData);
        }
    });
}
function search() {
    var keyword = $('#searchText').val();
    $.ajax({
        url: "../BBS/GetSearchResult",
        method: 'Get',
        dataType: 'json',
        cache: false,
        data: {
            keyword: keyword
        },
        success: function (e) {
            if (e.success) {
                var html = '';
                $.each(e.retData, function (index) {
                    html += '<li>\
                                <a href="/BBS/Details/' + e.retData[index].Id + '">\
                                    <div class="row">\
                                        <div class="col-xs-1">\
                                            <img style="margin-left:10px" src="' + e.retData[index].UserImage + '" width="30" />\
                                        </div>\
                                        <div class="col-xs-9">\
                                            <p class="bbsTitle">'+ e.retData[index].Title + '</p>\
                                        </div>\
                                    </div>\
                                    <p style="margin-left:60px">' + e.retData[index].Sample + '</p>\
                                </a>\
                            </li>';
                });
                $('#bbsList').html(html);
            }
            else {
                layer.msg(e.retData);
            }
        },
        error: function (e) {
            layer.msg(e.retData);
        }

    });
}