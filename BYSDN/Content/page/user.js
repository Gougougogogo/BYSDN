$(document).ready(function () {
    UserManager.requestTimelineBody();
});

var UserManager = {};

UserManager.CurrentPage = 1;

UserManager.getMore = function () {
    UserManager.requestTimelineBody();
};

UserManager.requestTimelineBody = function () {
    $.ajax({
        url: '/User/GetTimeLineData',
        data: { page: UserManager.CurrentPage },
        cache: false,
        success: function (e) {
            var c = e;
            if (e.success) {
                if (e.retData.length > 0) {
                    var html = $('.cbp_tmtimeline').html();
                    $.each(e.retData, function (index) {
                        html += UserManager.getHtml(e.retData[index]);
                    });
                    $('.cbp_tmtimeline').html(html)
                    UserManager.CurrentPage++;
                }
                else {
                    layer.msg('No more data');
                }
            }
        }
    });
};

UserManager.getHtml = function (data) {
    var html = '';
    switch(data.Type)
    {
        case "New Reply":
            html = '<li>\
                        <time class="cbp_tmtime" datetime="2014-09-19T20:23"><span>' + data.ShortTime + '</span> <span>' + data.ShortDate + '</span></time>\
                        <div class="cbp_tmicon timeline-bg-info">\
                            <i class="fa-reply"></i>\
                        </div>\
                        <div class="cbp_tmlabel">\
                            <h2>Reply a question</h2>\
                            <blockquote><a onclick="UserManager.redirect(\'' + data.ID + '\')" style="cursor:pointer">' + data.BodyContent + '</a></blockquote>\
                        </div>\
                    </li>';
            break;
        case "New Question":
            html = '<li>\
                        <time class="cbp_tmtime" datetime="2014-09-19T20:23"><span>' + data.ShortTime + '</span> <span>' + data.ShortDate + '</span></time>\
                        <div class="cbp_tmicon timeline-bg-danger">\
                                    <i class="fa-paper-plane"></i>\
                        </div>\
                        <div class="cbp_tmlabel">\
                            <h2>Publish a question</h2>\
                            <blockquote><a onclick="UserManager.redirect(\'' + data.ID + '\')" style="cursor:pointer">' + data.BodyContent + '</a></blockquote>\
                        </div>\
                    </li>';
            break;
        case "New User":

            html = '<li>\
                       <time class="cbp_tmtime" datetime="2014-09-19T20:23"><span>' + data.ShortTime + '</span> <span>' + data.ShortDate + '</span></time>\
                        <div class="cbp_tmicon timeline-bg-success">\
                            <i class="fa-user"></i>\
                        </div>\
                        <div class="cbp_tmlabel">\
                            <h2>Regist</h2>\
                            <blockquote>' + data.BodyContent + '</blockquote>\
                        </div>\
                    </li>';

            break;
    }
    return html;
};

UserManager.redirect = function (id) {
    window.location = window.location.protocol + "//" + window.location.host + "/BBS/Details/" + id;
}